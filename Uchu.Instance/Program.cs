using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Auth.Handlers;
using Uchu.Char.Handlers;
using Uchu.Core;
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

            ServerSpecification specification;

            await using (var ctx = new UchuContext())
            {
                specification = await ctx.Specifications.FirstOrDefaultAsync(c => c.Id == id);

                if (specification == default)
                    throw new ArgumentException($"{args[0]} is not a valid server specification ID");
            }

            var server = specification.ServerType == ServerType.World
                ? new WorldServer(specification)
                : new Server(specification.Id);
            
            await server.ConfigureAsync(args[1]);

            try
            {
                switch (specification.ServerType)
                {
                    case ServerType.Authentication:
                        await server.StartAsync(typeof(LoginHandler).Assembly, true);
                        break;
                    case ServerType.Character:
                        await server.StartAsync(typeof(CharacterHandler).Assembly);
                        break;
                    case ServerType.World:
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

            try
            {
                await using var ctx = new UchuContext();
                
                specification = await ctx.Specifications.FirstAsync(c => c.Id == id);

                ctx.Specifications.Remove(specification);

                await ctx.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }

            Console.ReadKey();
        }
    }
}