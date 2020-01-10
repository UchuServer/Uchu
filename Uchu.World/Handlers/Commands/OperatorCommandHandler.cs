using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World.Handlers.Commands
{
    public class OperatorCommandHandler : HandlerGroup
    {
        [CommandHandler(Signature = "stop", Help = "Stop the server", GameMasterLevel = GameMasterLevel.Operator)]
        public async Task<string> Stop(string[] arguments, Player player)
        {
            await using var ctx = new UchuContext();

            var world = await ctx.Specifications.FirstOrDefaultAsync(
                s => s.ServerType == ServerType.World && s.Id != Server.Id
            );

            foreach (var zonePlayer in player.Zone.Players)
            {
                if (world == default)
                {
                    zonePlayer.Message(new DisconnectNotifyPacket
                    {
                        DisconnectId = DisconnectId.ServerShutdown
                    });
                    
                    continue;
                }
                
                zonePlayer.SendChatMessage($"This zone is closing, going to {world.ZoneId}!");
                
                await zonePlayer.SendToWorldAsync(world);
            }

            var delay = 1000;

            if (arguments.Length > 0)
            {
                int.TryParse(arguments[0], out delay);
            }

            await Task.Delay(delay);

            Environment.Exit(0);

            return "Stopped server";
        }
    }
}