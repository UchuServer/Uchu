using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World
{
    public class ChatHandler : HandlerGroupBase
    {
        [PacketHandler]
        public async Task ChatMessage(ClientChatMessagePacket packet, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FindAsync(session.CharacterId);

                Console.WriteLine($"(chat) {character.Name}: {packet.Message}");

                Server.Send(new ChatMessagePacket
                {
                    Message = packet.Message,
                    SenderName = character.Name,
                    SenderObjectId = (ulong) session.CharacterId
                }, endpoint);
            }
        }

        [PacketHandler]
        public async Task CommandMessage(ParseChatMessageMessage msg, IPEndPoint endpoint)
        {
            var session = Server.SessionCache.GetSession(endpoint);

            if (!msg.Message.StartsWith('/'))
                return;

            var message = msg.Message.Remove(0, 1);

            var command = message.Split(' ')[0];
            var args = message.Split(' ').ToList();

            args.RemoveAt(0);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FindAsync(session.CharacterId);

                Console.WriteLine($"(chat) {character.Name}: {msg.Message}");

                Server.Send(new ChatMessagePacket
                {
                    Message = $"Unknown command: {command}\0"
                }, endpoint);
            }
        }
    }
}