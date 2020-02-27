using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Auth.Handlers;
using Uchu.Char.Handlers;
using Uchu.Core;
using Uchu.Core.Providers;
using Uchu.World;
using Uchu.World.Handlers;

namespace Uchu.Instance
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Expected 2 argument.");

            if (!Guid.TryParse(args[0], out var id))
                throw new ArgumentException($"{args[0]} is not a valid GUID");
            
            var serializer = new XmlSerializer(typeof(Configuration));

            if (!File.Exists(args[1]))
            {
                throw new ArgumentException($"{args[1]} config file does not exist.");
            }

            Configuration configuration;
            
            await using (var fs = File.OpenRead(args[1]))
            {
                UchuContextBase.Config = configuration = (Configuration) serializer.Deserialize(fs);
            }
            
            var masterPath = Path.GetDirectoryName(args[1]);

            SqliteContext.DatabasePath = Path.Combine(masterPath, "./Uchu.sqlite");

            var api = new ApiManager(configuration.ApiConfig.Protocol, configuration.ApiConfig.Domain);

            var instance = await api.RunCommandAsync<InstanceInfoResponse>(
                configuration.ApiConfig.Port, $"instance/target?i={id}"
            ).ConfigureAwait(false);

            if (!instance.Success)
            {
                Logger.Error(instance.FailedReason);

                throw new Exception(instance.FailedReason);
            }
            
            var server = instance.Info.Type == (int) ServerType.World
                ? new WorldServer(id)
                : new Server(id);

            Console.Title = $"{(ServerType) instance.Info.Type}:{instance.Info.Port}";
            
            await server.ConfigureAsync(args[1]);

            try
            {
                switch ((ServerType) instance.Info.Type)
                {
                    case ServerType.Authentication:
                        await server.StartAsync(typeof(LoginHandler).Assembly, true);
                        break;
                    case ServerType.Character:
                        await server.StartAsync(typeof(CharacterHandler).Assembly);
                        break;
                    case ServerType.World:
                        server.RegisterAssembly(typeof(CharacterHandler).Assembly);
                        await server.StartAsync(typeof(WorldInitializationHandler).Assembly);
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
    }
}