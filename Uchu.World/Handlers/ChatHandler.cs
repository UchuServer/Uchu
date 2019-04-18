using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Scriptable;
using Uchu.World.Scriptable;

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
            var world = Server.Worlds[(ZoneId) session.ZoneId];

            if (!msg.Message.StartsWith('/'))
                return;

            var message = msg.Message.Remove(0, 1);

            var command = message.Split(' ')[0].ToLower();
            var args = message.Split(' ').ToList();

            args.RemoveAt(0);

            /*
             * Check for one of the hard-coded commands. TODO: Make them, not hard coded.
             */
            
            string chatCallback;

            Console.WriteLine(
                $"Command: {world.Players.First(p => p.CharacterId == session.CharacterId).ReplicaPacket.Name}: {msg.Message}");
            var player = world.Players.First(p => p.CharacterId == session.CharacterId);
            switch (command)
            {
                case "give":
                    chatCallback = await ChatCommands.GiveCommand(args.ToArray(), player);
                    break;
                case "fly":
                    chatCallback = ChatCommands.FlyCommand(args.ToArray(), player);
                    break;
                case "state":
                    chatCallback = ChatCommands.StateCommand(args.ToArray(), player);
                    break;
                default:
                    chatCallback = $"Unknown command: {command}\0";
                    break;
            }

            Server.Send(new ChatMessagePacket
            {
                Message = chatCallback
            }, endpoint);
        }
    }
}