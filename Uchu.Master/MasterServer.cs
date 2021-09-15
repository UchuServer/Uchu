using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
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
                await using var uchuContext = new UchuContext();
                await uchuContext.EnsureUpdatedAsync();
                
                await using var cdClientContext = new CdClientContext();
                await cdClientContext.EnsureUpdatedAsync();

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

        /// <summary>
        /// Attempt to find the resource folder of an unpacked client installed using Nexus LU Launcher.
        /// </summary>
        /// <returns>The path to the client's res folder.</returns>
        /// <exception cref="FileNotFoundException">Valid resource folder was not found.</exception>
        private static string FindNlulClientResources()
        {
            // Get .nlul location, e.g. ~/.nlul
            var nlulHomeLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nlul");
            if (!Directory.Exists(nlulHomeLocation))
                throw new DirectoryNotFoundException("No NLUL installation found.");

            // Get path to NLUL config file
            var launcherFileLocation = Path.Combine(
                nlulHomeLocation,
                "launcher.json");
            if (!File.Exists(launcherFileLocation))
                throw new FileNotFoundException("No NLUL configuration file found.");

            // Parse NLUL config file
            var nlulConfig = JsonDocument.Parse(File.ReadAllText(launcherFileLocation));

            // Client parent directory is ClientParentLocation if set, otherwise nlulHomeLocation
            var clientParentLocation = nlulHomeLocation;
            if (nlulConfig.RootElement.TryGetProperty("ClientParentLocation", out var customClientParentLocation))
                clientParentLocation = customClientParentLocation.GetString();
            if (!Directory.Exists(clientParentLocation))
                throw new FileNotFoundException("Configured ClientParentLocation directory does not exist.");

            // Iterate over subdirectories to search for a valid client
            Debug.Assert(clientParentLocation != null, nameof(clientParentLocation) + " != null");
            foreach (var clientDirectory in Directory.GetDirectories(clientParentLocation))
            {
                var resLocation = Path.Combine(clientDirectory, "res");
                if (!EnsureUnpackedClient(resLocation))
                    continue;

                return resLocation;
            }

            throw new FileNotFoundException("No unpacked client found.");
        }

        /// <summary>
        /// Load the server configuration.
        /// </summary>
        /// <exception cref="DirectoryNotFoundException">Resource folder is not configured correctly.</exception>
        /// <exception cref="FileNotFoundException">One of the required files are not configured correctly.</exception>
        private static async Task ConfigureAsync()
        {
            const string configFilename = "config.xml";
            const string legacySecondConfigName = "config.default.xml";

            // Use config.xml if it exists
            if (File.Exists(configFilename))
            {
                Config = UchuConfiguration.Load(configFilename);
                Logger.SetConfiguration(Config);
                Logger.SetServerTypeInformation("Master");
            }
            // Otherwise, use config.default.xml if it exists
            else if (File.Exists(legacySecondConfigName))
            {
                Config = UchuConfiguration.Load(legacySecondConfigName);
                Logger.SetConfiguration(Config);
                Logger.SetServerTypeInformation("Master");
            }
            // Otherwise, generate a new config file
            else
            {
                Config = new UchuConfiguration();
                Logger.SetConfiguration(Config);
                Logger.SetServerTypeInformation("Master");

                // Add default value for instance DLL source and script DLL source.
                if (File.Exists("lib/Uchu.Instance.dll"))
                {
                    Config.DllSource.Instance = "lib/Uchu.Instance.dll";
                }
                Config.DllSource.ScriptDllSource.Add(File.Exists("lib/Uchu.StandardScripts.dll")
                    ? "lib/Uchu.StandardScripts.dll"
                    : "../../../../Uchu.StandardScripts/bin/Debug/net5.0/Uchu.StandardScripts.dll");

                // Write config file
                Config.Save(configFilename);

                var info = new FileInfo(configFilename);

                Logger.Warning($"No config file found, created one at {info.FullName}");
            }

            SqliteContext.DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "./Uchu.sqlite");

            UchuContextBase.Config = Config;

            UseAuthentication = Config.Networking.HostAuthentication;

            // Check: resource folder
            var resourceFolder = Config.ResourcesConfiguration.GameResourceFolder;

            if (!EnsureUnpackedClient(resourceFolder))
            {
                // Try finding NLUL client
                try
                {
                    Config.ResourcesConfiguration.GameResourceFolder = FindNlulClientResources();
                    Logger.Information($"Using automatically detected client resource folder: {Config.ResourcesConfiguration.GameResourceFolder}");
                }
                catch
                {
                    // Unsuccessful in finding unpacked client resource folder
                    throw new DirectoryNotFoundException(
                        "Please enter a valid unpacked client resource folder in the configuration file.");
                }
            }

            // Check: Uchu.Instance.dll
            DllLocation = Config.DllSource.Instance;

            if (!File.Exists(DllLocation))
            {
                throw new FileNotFoundException("Could not find file specified in <Instance> under <DllSource> in config.xml.");
            }

            // Check: Uchu.StandardScripts.dll
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

        /// <summary>
        /// Check whether a directory is the res folder of an unpacked client. Accounts for the possibility of the
        /// directory not existing at all.
        /// </summary>
        /// <param name="directory">Path to the directory.</param>
        /// <returns>Whether it's an unpacked res folder.</returns>
        private static bool EnsureUnpackedClient(string directory)
        {
            return !string.IsNullOrWhiteSpace(directory) &&
                   directory.EndsWith("res") &&
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
