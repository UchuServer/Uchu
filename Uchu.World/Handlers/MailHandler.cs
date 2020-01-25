using System;
using System.Threading.Tasks;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class MailHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ClientMailPacketHandler(ClientMailPacket packet, IRakConnection connection)
        {
            var player = Server.FindPlayer(connection);

            if (player == default)
            {
                Logger.Error($"Could not find player for: {connection}");
                
                return;
            }

            switch (packet.Id)
            {
                case ClientMailPacket.MailPacketId.Send:
                    await SendHandler(packet.MailStruct as ClientMailPacket.MailSend, player);
                    break;
                case ClientMailPacket.MailPacketId.DataRequest:
                    await DataRequestHandler(player);
                    break;
                case ClientMailPacket.MailPacketId.AttachmentCollected:
                    await AttachmentCollectHandler(packet.MailStruct as ClientMailPacket.MailAttachmentCollected, player);
                    break;
                case ClientMailPacket.MailPacketId.Delete:
                    await DeleteHandler(packet.MailStruct as ClientMailPacket.MailDelete, player);
                    break;
                case ClientMailPacket.MailPacketId.Read:
                    await ReadHandler(packet.MailStruct as ClientMailPacket.MailRead, player);
                    break;
                case ClientMailPacket.MailPacketId.NotificationRequest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static async Task SendHandler(ClientMailPacket.MailSend packet, Player player)
        {
            
        }

        public static async Task DataRequestHandler(Player player)
        {
            
        }

        public static async Task AttachmentCollectHandler(ClientMailPacket.MailAttachmentCollected packet, Player player)
        {
            
        }

        public static async Task DeleteHandler(ClientMailPacket.MailDelete packet, Player player)
        {
            
        }

        public static async Task ReadHandler(ClientMailPacket.MailRead packet, Player player)
        {
            
        }
    }
}