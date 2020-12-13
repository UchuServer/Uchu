using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class ServerSideEventHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task EventHandler(FireServerEventMessage message, Player player)
        {
            await message.Sender.OnFireServerEvent.InvokeAsync(message.Arguments, message);
            await player.OnFireServerEvent.InvokeAsync(message.Arguments, message);
        }
    }
}