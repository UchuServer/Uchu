using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World.Chat;

namespace Uchu.World.Handlers
{
    public class ChatHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ParseChatMessageHandler(ParseChatMessage message, Player player)
        {
            if (!message.Message.StartsWith('/'))
                return;
            
            var command = message.Message.Remove(0, 1);

            var response = await ((WorldServer) Server).AdminCommand(command, player);

            Server.Send(new ChatMessagePacket
            {
                Message = $"{response}\0"
            }, player.EndPoint);
        }
    }
}