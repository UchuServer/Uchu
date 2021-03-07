using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Config;
using Uchu.Core.Providers;
using Uchu.Master.Api;

namespace Uchu.Master
{
    internal static class MasterServer
    {
        public static string DllLocation { get; set; }

        public static UchuConfiguration Config { get; set; }

        public static List<ServerInstance> Instances { get; set; }

        public static Dictionary<string, int> InstanceHeartBeats { get; set; }
        public static Dictionary<string, ServerHealth> InstanceHealth { get; set; }
        public static Timer InstanceHeartBeatCheck { get; set; }  
        public static string ConfigPath { get; set; }
        
        public static string CdClientPath { get; set; }
        
        public static bool Running { get; set; }
        
        public static bool UseAuthentication { get; set; }
        
        public static int ApiPortIndex { get; set; }
        
        public static int ApiPort { get; set; }
        
        public static bool IsSubsidiary { get; set; }
        
        public static List<int> Subsidiaries { get; set; }
        
        public static ApiManager Api { get; set; }
        
        public static int WorldServerHeartBeatsIntervalInMinutes { get; set; }
        public static int WorldServerHeartBeatsPerInterval { get; set; }

        public static int MasterPort => Config.ApiConfig.Port;
        
        private static async Task Main(string[] args)
        {
            Subsidiaries = new List<int>();
            Instances = new List<ServerInstance>();
            InstanceHeartBeats = new Dictionary<string, int>();
            InstanceHealth = new Dictionary<string, ServerHealth>();
            
            try
            {
                await ConfigureAsync();
            }
            catch (Exception e) when (e is FileNotFoundException || e is DirectoryNotFoundException)
            {
                Logger.Error(e.Message);
                if (Config.ServerBehaviour.PressKeyToExit)
                {
                    Logger.Error("Press any key to exit...");
                    Console.ReadKey();
                }
                Environment.Exit(1);
            }
            
            var databaseVerified = await CheckForDatabaseUpdatesAsync();
            if (!databaseVerified)
            {
                Logger.Error($"Failed to connect to database provider \"{Config.Database.Provider}\".");
                if (Config.ServerBehaviour.PressKeyToExit)
                {
                    Logger.Error("Press any key to exit...");
                    Console.ReadKey();
                }
                Environment.Exit(1);
            }
            
            Console.CancelKeyPress += ShutdownProcesses;

            AppDomain.CurrentDomain.ProcessExit += ShutdownProcesses;
            
            await SetupApiAsync();
            
            // Setup health checks and automatic server closing
            InstanceHeartBeatCheck = new Timer(WorldServerHeartBeatsIntervalInMinutes * 60000);
            InstanceHeartBeatCheck.Elapsed += async (sender, eventArgs) =>
            {
                foreach (var instance in Instances.Where(i => i.ServerType == ServerType.World).ToList())
                {
                    var id = instance.Id.ToString();
                    if (InstanceHealth.ContainsKey(id) && InstanceHeartBeats.ContainsKey(id))
                    {
                        InstanceHealth[id] = (InstanceHeartBeats[id] != 0 
                                ? (float) InstanceHeartBeats[id] / WorldServerHeartBeatsPerInterval : 0) switch
                        {
                            var health when health >= 1 => ServerHealth.Healthy,
                            var health when health >= 0.75 => ServerHealth.Lagging,
                            var health when health >= 0.5 => ServerHealth.SeverelyLagging,
                            var health when health >= 0.25 => ServerHealth.Unhealthy,
                            
                            // If hardly any heartbeats were received, gradually downgrade the health until closing
                            _ => (ServerHealth) (int) InstanceHealth[id] - 1
                        };
                    }
                    else
                    {
                        InstanceHealth.Add(id, ServerHealth.Dead);
                    }

                    Logger.Information($"{id} is {InstanceHealth[id]}");
                    InstanceHeartBeats[id] = 0;
                }
                
                // Kill all unhealthy instances
                foreach (var unhealthyInstance in Instances.Where(i => i.ServerType == ServerType.World
                                                                       && InstanceHealth.TryGetValue(i.Id.ToString(),
                                                                           out var health)
                                                                       && health == ServerHealth.Dead).ToList())
                {
                    Logger.Information($"Closing {unhealthyInstance.Id.ToString()} " +
                                       $"({InstanceHealth[unhealthyInstance.Id.ToString()]}) due to health status.");
                    
                    await Api.RunCommandAsync<BaseResponse>(ApiPort, 
                        $"instance/decommission?instance={unhealthyInstance.Id.ToString()}");
                }
            };
            
            InstanceHeartBeatCheck.Enabled = true;
            InstanceHeartBeatCheck.AutoReset = true;
            
            try
            {
                // Throws HttpListenerException when port is unavailable or permission is denied
                await Api.StartAsync(ApiPort);
            }
            catch (HttpListenerException e)
            {
                Logger.Error($"Could not start API listener on port {ApiPort}: {e.Message}. Try specifying a different port in config.xml.");
                if (Config.ServerBehaviour.PressKeyToExit)
                {
                    Logger.Error("Press any key to exit...");
                    Console.ReadKey();
                }
                Environment.Exit(1);
            }
        }

        private static async Task<bool> CheckForDatabaseUpdatesAsync()
        {
            try
            {
                await using var ctx = new UchuContext();
                
                await ctx.EnsureUpdatedAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void ShutdownProcesses(object _, EventArgs ev)
        {
            Running = false;

            foreach (var instance in Instances)
            {
                instance.Process.Kill();
            }

            if (!(ev is ConsoleCancelEventArgs cancelEv)) return;
            
            cancelEv.Cancel = true;

            Environment.Exit(0);
        }

        private static async Task SetupApiAsync()
        {
            var apiConfig = Config.ApiConfig;

            Api = new ApiManager(apiConfig.Protocol, apiConfig.Domain);
            
            Api.RegisterCommandCollection<AccountCommands>();
            
            Api.RegisterCommandCollection<MasterCommands>();

            Api.RegisterCommandCollection<CharacterCommands>();

            var response = await Api.RunCommandAsync<ClaimPortResponse>(MasterPort, "subsidiary");

            if (response != null && response.Success)
            {
                IsSubsidiary = true;
                
                ApiPort = response.Port;

                Logger.Information($"Is subsidiary: {ApiPort}");
            }
            else
            {
                ApiPort = MasterPort;
            }

            Api.OnLoaded += async () =>
            {
                Logger.Information($"API Ready!");
                
                await StartDefaultInstances();
            };
        }

        private static async Task StartDefaultInstances()
        {
            var hostAuthentication = Config.Networking.HostAuthentication;

            var hostCharacter = Config.Networking.HostCharacter;

            if (IsSubsidiary)
            {
                var instances = await GetAllInstancesAsync();
                
                if (hostAuthentication)
                {
                    if (instances.Any(i => i.Type == (int) ServerType.Authentication))
                    {
                        hostAuthentication = false;
                    }
                }

                if (hostCharacter)
                {
                    if (instances.Any(i => i.Type == (int) ServerType.Character))
                    {
                        hostCharacter = false;
                    }
                }
            }

            if (hostAuthentication)
            {
                await StartInstanceAsync(ServerType.Authentication, Config.Networking.AuthenticationPort);
            }

            if (hostCharacter)
            {
                await StartInstanceAsync(ServerType.Character, Config.Networking.CharacterPort);
            }
        }

        private static async Task ConfigureAsync()
        {
            var serializer = new XmlSerializer(typeof(UchuConfiguration));
            var configFilename = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            if (File.Exists(configFilename))
            {
                await using var file = File.OpenRead(configFilename);
                LogQueue.Config = Config = (UchuConfiguration) serializer.Deserialize(file);
            }
            else
            {
                LogQueue.Config = Config = new UchuConfiguration();

                Config.DllSource.ScriptDllSource.Add("Enter path to Uchu.StandardScripts.dll");

                var backup = File.CreateText("config.default.xml");

                serializer.Serialize(backup, Config);

                backup.Close();

                Logger.Warning("No config file found, creating default.");

                var info = new FileInfo("config.default.xml");

                Logger.Information($"You may now continue with configuring Uchu. Default config file located at: {info.FullName}");

                throw new FileNotFoundException("No config file found.", info.FullName);
            }

            SqliteContext.DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "./Uchu.sqlite");

            UchuContextBase.Config = Config;

            var resourceFolder = Config.ResourcesConfiguration?.GameResourceFolder;

            if (!string.IsNullOrWhiteSpace(resourceFolder))
            {
                if (EnsureUnpackedClient(resourceFolder))
                {
                    Logger.Information($"Using game resources: {Config.ResourcesConfiguration.GameResourceFolder}");
                }
                else
                {
                    Logger.Error($"Invalid local resources (Invalid path or no .luz files found). Please ensure you are using an unpacked client.");

                    throw new FileNotFoundException("No luz files found.");
                }
            }
            else
            {
                Logger.Error("No input location of local resources. Please input in config file.");

                throw new DirectoryNotFoundException("No local resource path.");
            }

            UseAuthentication = Config.Networking.HostAuthentication;

            DllLocation = Config.DllSource.Instance;

            if (!File.Exists(DllLocation))
            {
                throw new FileNotFoundException("Could not find file specified in <Instance> under <DllSource> in config.xml.");
            }

            var validScriptPackExists = false;

            Config.DllSource.ScriptDllSource.ForEach(scriptPackPath =>
            {
                if (File.Exists(scriptPackPath))
                {
                    Logger.Information($"Using script pack: {scriptPackPath}");
                    validScriptPackExists = true;
                    return;
                }

                Logger.Warning($"Could not find script pack at {scriptPackPath}");
            });

            if (!validScriptPackExists)
            {
                throw new FileNotFoundException("No valid <ScriptDllSource> specified under <DllSource> in config.xml.\n"
                                                + "Without Uchu.StandardScripts.dll, Uchu cannot function correctly.");
            }

            ApiPortIndex = Config.ApiConfig.Port;
            WorldServerHeartBeatsPerInterval = Config.Networking.WorldServerHeartBeatsPerInterval;
            WorldServerHeartBeatsIntervalInMinutes = Config.Networking.WorldServerHeartBeatIntervalInMinutes;

            var source = Directory.GetCurrentDirectory();

            ConfigPath = Path.Combine(source, $"{configFilename}");
            CdClientPath = Path.Combine(source, "CDClient.db");
            
            Logger.Information($"Using configuration: {ConfigPath}\nUsing CDClient: {CdClientPath}");
        }

        private static bool EnsureUnpackedClient(string directory)
        {
            return directory.EndsWith("res") &&
                   Directory.Exists(directory) &&
                   Directory.GetFiles(directory, "*.luz", SearchOption.AllDirectories).Any();
        }

        public static async Task<Guid> StartInstanceAsync(ServerType type, int port)
        {
            var id = Guid.NewGuid();
            
            var instance = new ServerInstance(id)
            {
                ServerType = type,
                ServerPort = port,
                ApiPort = await ClaimApiPortAsync(),
            };

            instance.Start(DllLocation, Config.DllSource.DotNetPath);
            
            Instances.Add(instance);

            if (type == ServerType.World)
            {
                InstanceHeartBeats.Add(id.ToString(), 0);
                InstanceHealth.Add(id.ToString(), ServerHealth.Healthy);
            }

            return id;
        }

        public static async Task<int> ClaimWorldPortAsync()
        {
            if (IsSubsidiary)
            {
                var response = await Api.RunCommandAsync<ClaimPortResponse>(
                    MasterPort, "claim/world"
                );

                if (response.Success) return response.Port;
                
                Logger.Error(response.FailedReason);

                throw new Exception(response.FailedReason);
            }

            var instances = await GetAllInstancesAsync();

            var worlds = instances.Where(i => i.Type == (int) ServerType.World).ToArray();

            var specified = Config.Networking.WorldPorts.Count > 0;

            lock (Api)
            {
                if (!specified)
                {
                    if (worlds.Length == default) return 20000;
                    
                    return worlds.Max(i => i.Port) + 1;
                }
                
                var available = Config.Networking.WorldPorts.ToList();

                foreach (var world in worlds)
                {
                    if (available.Contains(world.Port))
                    {
                        available.Remove(world.Port);
                    }
                }

                return available.FirstOrDefault();
            }
        }

        public static async Task<int> ClaimApiPortAsync()
        {
            if (IsSubsidiary)
            {
                var response = await Api.RunCommandAsync<ClaimPortResponse>(
                    MasterPort, "claim/api"
                );

                if (response.Success) return response.Port;
                
                Logger.Error(response.FailedReason);

                throw new Exception(response.FailedReason);
            }
            
            lock (Api)
            {
                return ++ApiPortIndex;
            }
        }

        public static async Task<List<InstanceInfo>> GetAllInstancesAsync()
        {
            if (IsSubsidiary)
            {
                var result = await Api.RunCommandAsync<InstanceListResponse>(
                    MasterPort, "instance/list/complete"
                );

                return result.Instances;
            }
            
            var localList = await Api.RunCommandAsync<InstanceListResponse>(
                ApiPort, "instance/list"
            );

            var instances = localList.Instances;

            foreach (var subsidiary in Subsidiaries)
            {
                Logger.Information($"Request instance from: {subsidiary}");
                
                var result = await Api.RunCommandAsync<InstanceListResponse>(
                    subsidiary, "instance/list"
                );
                
                if (result == null)
                    continue;

                if (!result.Success)
                {
                    Logger.Error(result.FailedReason);
                    continue;
                }

                instances.AddRange(result.Instances);
            }

            return instances;
        }
    }
}