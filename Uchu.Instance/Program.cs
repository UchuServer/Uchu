using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Auth.Handlers;
using Uchu.Char.Handlers;
using Uchu.Core;
using Uchu.Core.Config;
using Uchu.Core.Providers;
using Uchu.World;
using Uchu.World.Handlers;

namespace Uchu.Instance
{
    internal static class Program
    {
        private static UchuServer UchuServer { get; set; }
        
        private static Guid Id { get; set; }
        
        private static ServerType ServerType { get; set; }
        
        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Expected 2 argument.");

            if (!Guid.TryParse(args[0], out var id))
                throw new ArgumentException($"{args[0]} is not a valid GUID");

            Id = id;

            await ConfigureAsync(args[1]).ConfigureAwait(false);

            Logger.Debug($"Process ID: {Process.GetCurrentProcess().Id}");
            
            try
            {
                switch (ServerType)
                {
                    case ServerType.Authentication:
                        await UchuServer.StartAsync(typeof(LoginHandler).Assembly, true);
                        break;
                    case ServerType.Character:
                        await UchuServer.StartAsync(typeof(CharacterHandler).Assembly);
                        break;
                    case ServerType.World:
                        UchuServer.RegisterAssembly(typeof(CharacterHandler).Assembly);
                        await UchuServer.StartAsync(typeof(WorldInitializationHandler).Assembly);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
            
            Logger.Information("Exiting...");

            Console.ReadKey();
        }

        private static async Task ConfigureAsync(string config)
        {
            var serializer = new XmlSerializer(typeof(UchuConfiguration));

            if (!File.Exists(config))
            {
                throw new ArgumentException($"{config} config file does not exist.");
            }

            UchuConfiguration uchuConfiguration;
            
            await using (var fs = File.OpenRead(config))
            {
                UchuContextBase.Config = uchuConfiguration = (UchuConfiguration) serializer.Deserialize(fs);
            }
            
            var masterPath = Path.GetDirectoryName(config);

            SqliteContext.DatabasePath = Path.Combine(masterPath, "./Uchu.sqlite");

            var api = new ApiManager(uchuConfiguration.ApiConfig.Protocol, uchuConfiguration.ApiConfig.Domain);

            var instance = await api.RunCommandAsync<InstanceInfoResponse>(
                uchuConfiguration.ApiConfig.Port, $"instance/target?i={Id}"
            ).ConfigureAwait(false);

            if (!instance.Success)
            {
                Logger.Error(instance.FailedReason);

                throw new Exception(instance.FailedReason);
            }

            UchuServer = instance.Info.Type == (int) ServerType.World
                ? new WorldUchuServer(Id)
                : new UchuServer(Id);
            
            Console.Title = $"{(ServerType) instance.Info.Type}:{instance.Info.Port}";

            ServerType = (ServerType) instance.Info.Type;
            
            await UchuServer.ConfigureAsync(config);
        }
    }
}