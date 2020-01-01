using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.Master
{
    internal static class MasterServer
    {
        public static string DllLocation { get; set; }

        public static Configuration Config { get; set; }
        
        public static ManagedServer AuthenticationServer { get; set; }
        
        public static ManagedServer CharacterServer { get; set; }
        
        public static List<ManagedWorldServer> WorldServers { get; set; } = new List<ManagedWorldServer>();
        
        public static string ConfigPath { get; set; }
        
        public static string CdClientPath { get; set; }
        
        private static async Task Main(string[] args)
        {
            await OpenConfig();
            
            await using (var ctx = new UchuContext())
            {
                Logger.Information("Checking for database updates...");

                await ctx.EnsureUpdatedAsync();
                
                Logger.Information("Database up to date...");
                
                foreach (var specification in ctx.Specifications)
                {
                    ctx.Specifications.Remove(specification);
                }

                foreach (var request in ctx.WorldServerRequests)
                {
                    ctx.WorldServerRequests.Remove(request);
                }

                foreach (var session in ctx.SessionCaches)
                {
                    ctx.SessionCaches.Remove(session);
                }

                await ctx.SaveChangesAsync();
            }

            await StartAuthentication();
            await StartCharacter();

            Console.CancelKeyPress += ShutdownProcesses;

            AppDomain.CurrentDomain.ProcessExit += ShutdownProcesses;

            await HandleRequests();
        }

        private static void ShutdownProcesses(object _, EventArgs ev)
        {
            if (!AuthenticationServer.Process.HasExited)
                AuthenticationServer.Process.Kill();

            if (!CharacterServer.Process.HasExited)
                CharacterServer.Process.Kill();

            foreach (var server in WorldServers.Where(server => !server.Process.HasExited))
            {
                server.Process.Kill();
            }

            if (!(ev is ConsoleCancelEventArgs cancelEv)) return;
            
            cancelEv.Cancel = true;

            Environment.Exit(0);
        }

        private static async Task HandleRequests()
        {
            while (true)
            {
                await Task.Delay(50);

                await using var ctx = new UchuContext();

                //
                // Cleanup
                //
                
                for (var index = 0; index < WorldServers.Count; index++)
                {
                    var worldServer = WorldServers[index];
                    
                    if (worldServer.Process.HasExited)
                    {
                        WorldServers.RemoveAt(index);
                    }
                    else
                    {
                        var specifications = await ctx.Specifications.FirstAsync(w => w.Id == worldServer.Id);

                        if (specifications.ActiveUserCount != default)
                        {
                            worldServer.EmptyTime = default;
                            
                            continue;
                        }
                        
                        worldServer.EmptyTime++;
                        
                        if (worldServer.EmptyTime != 10000) continue;

                        // Evil, but works
                        worldServer.Process.Kill();
                        
                        WorldServers.RemoveAt(index);
                    }
                }

                foreach (var request in ctx.WorldServerRequests)
                {
                    if (request.State == WorldServerRequestState.Unanswered)
                    {
                        //
                        // Search for available server
                        //
                        
                        foreach (var worldServer in WorldServers.Where(w => w.ZoneId == request.ZoneId))
                        {
                            var specification = await ctx.Specifications.FirstAsync(s => s.Id == worldServer.Id);

                            if (specification.ActiveUserCount >= specification.MaxUserCount) continue;

                            await using (var scopeCtx = new UchuContext())
                            {
                                var req = await scopeCtx.WorldServerRequests.FirstAsync(r => r.Id == request.Id);
                            
                                req.SpecificationId = specification.Id;

                                req.State = WorldServerRequestState.Complete;

                                await scopeCtx.SaveChangesAsync();
                            }

                            goto continueToNext;
                        }
                        
                        //
                        // Start new server
                        //
                        
                        ushort instanceId = 0;

                        for (var i = 0; i < ushort.MaxValue; i++)
                        {
                            if (WorldServers.Any(w => w.InstanceId == instanceId))
                            {
                                instanceId++;
                                
                                continue;
                            }

                            break;
                        }

                        await using (var scopeCtx = new UchuContext())
                        {
                            var req = await scopeCtx.WorldServerRequests.FirstAsync(r => r.Id == request.Id);

                            req.SpecificationId = await StartWorld(request.ZoneId, default, instanceId);
                        
                            req.State = WorldServerRequestState.Answered;
                            
                            await scopeCtx.SaveChangesAsync();
                        }
                    }
                    
                    continueToNext: ;
                }
            }
        }

        private static async Task OpenConfig()
        {
            var serializer = new XmlSerializer(typeof(Configuration));
            var fn = File.Exists("config.xml") ? "config.xml" : "config.default.xml";

            if (File.Exists(fn))
            {
                await using var file = File.OpenRead(fn);
                Logger.Config = Config = (Configuration) serializer.Deserialize(file);
            }
            else
            {
                Logger.Config = Config = new Configuration();

                var backup = File.CreateText("config.default.xml");

                serializer.Serialize(backup, Config);

                backup.Close();

                Logger.Warning("No config file found, creating default.");
            }

            if (!string.IsNullOrWhiteSpace(Config.ResourcesConfiguration?.GameResourceFolder))
            {
                Logger.Information($"Using Local Resources at `{Config.ResourcesConfiguration.GameResourceFolder}`");
            }

            var searchPath = Path.Combine($"{Directory.GetCurrentDirectory()}", Config.DllSource.ServerDllSourcePath);

            var matchStr = NormalizePath("/bin/");

            var files = Directory.GetFiles(searchPath, "*.dll", SearchOption.AllDirectories)
                .Select(Path.GetFullPath)
                .Where(f => f.Contains(matchStr)) // hacky solution
                .ToArray();

            foreach (var file in files)
            {
                switch (Path.GetFileName(file))
                {
                    case "Uchu.Instance.dll":
                        DllLocation = file;
                        break;
                    default:
                        continue;
                }
            }

            if (DllLocation == default)
            {
                throw new DllNotFoundException(
                    "Could not find DLL for Uchu.Instance. Did you forget to build it?"
                );
            }

            var source = Directory.GetCurrentDirectory();
            
            ConfigPath = Path.Combine(source, $"{fn}");
            CdClientPath = Path.Combine(source, "CDClient.db");
            
            Logger.Information($"{source}\n{ConfigPath}\n{CdClientPath}");
        }

        private static async Task StartAuthentication()
        {
            await using var ctx = new UchuContext();
            
            var id = Guid.NewGuid();

            await ctx.Specifications.AddAsync(new ServerSpecification
            {
                Id = id,
                Port = 21836,
                ServerType = ServerType.Authentication
            });

            AuthenticationServer = new ManagedServer(id, DllLocation, Config.DllSource.DotNetPath);

            await ctx.SaveChangesAsync();
        }
        
        private static async Task StartCharacter()
        {
            await using var ctx = new UchuContext();
            
            var id = Guid.NewGuid();

            await ctx.Specifications.AddAsync(new ServerSpecification
            {
                Id = id,
                Port = Config.Networking.CharacterPort,
                ServerType = ServerType.Character
            });

            CharacterServer = new ManagedServer(id, DllLocation, Config.DllSource.DotNetPath);
            
            await ctx.SaveChangesAsync();
        }
        
        private static async Task<Guid> StartWorld(ZoneId zoneId, uint cloneId, ushort instanceId)
        {
            await using var ctx = new UchuContext();
            
            var id = Guid.NewGuid();

            await ctx.Specifications.AddAsync(new ServerSpecification
            {
                Id = id,
                Port = 2003 + instanceId,
                ServerType = ServerType.World,
                ZoneId = zoneId,
                ZoneCloneId = cloneId,
                ZoneInstanceId = instanceId,
                MaxUserCount = 20
            });

            WorldServers.Add(new ManagedWorldServer(
                id,
                DllLocation,
                Config.DllSource.DotNetPath,
                zoneId,
                cloneId,
                instanceId
            ));
            
            await ctx.SaveChangesAsync();

            return id;
        }

        private static string NormalizePath(string path)
        {
            var toReplace = Path.DirectorySeparatorChar != '/' ? '/' : '\\';

            return path.Replace(toReplace, Path.DirectorySeparatorChar);
        }
    }
}