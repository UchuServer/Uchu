using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class MatchHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task MatchRequestHandler(MatchRequestMessage message, Player player)
        {
            player.SendChatMessage(
                $"Match Request:\nType: {message.Type}\nValue: {message.Value}\n{message.Activator}\n{message.Settings}",
                PlayerChatChannel.Normal
            );
        }
    }
}