using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using Uchu.Core;
using Uchu.Core.Client;
using static Uchu.World.ClientMailPacket;
using static Uchu.World.ServerMailPacket;

namespace Uchu.World.Handlers
{
    public class MailHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task ClientMailPacketHandler(ClientMailPacket packet, IRakConnection connection)
        {
            var player = UchuServer.FindPlayer(connection);
            if (player == default)
            {
                Logger.Error($"Could not find player for: {connection}");
                return;
            }

            switch (packet.Id)
            {
                case ClientMailPacketId.Send:
                    await SendHandler(packet.MailStruct as MailSend, player);
                    break;
                case ClientMailPacketId.DataRequest:
                    await DataRequestHandler(player);
                    break;
                case ClientMailPacketId.AttachmentCollected:
                    await AttachmentCollectHandler(packet.MailStruct as MailAttachmentCollected, player);
                    break;
                case ClientMailPacketId.Delete:
                    await DeleteHandler(packet.MailStruct as MailDelete, player);
                    break;
                case ClientMailPacketId.Read:
                    await ReadHandler(packet.MailStruct as MailRead, player);
                    break;
                case ClientMailPacketId.NotificationRequest:
                    await NotificationRequestHandler(player);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static async Task SendHandler(MailSend packet, Player player)
        {
            await using var ctx = new UchuContext();
            
            var response = new SendResponse();
            
            var recipient = await ctx.Characters.FirstOrDefaultAsync(c => c.Name == packet.RecipientName);
            if (recipient == default)
            {
                response.Code = MailResponseCode.RecipientNotFound;
                goto sendResponse;
            }

            var author = player.GetComponent<CharacterComponent>();
            if (recipient.Id == author.CharacterId)
            {
                response.Code = MailResponseCode.CannotMailYourself;
                goto sendResponse;
            }

            if (author.Currency < (long) packet.Currency)
            {
                response.Code = MailResponseCode.NotEnoughCurrency;
                goto sendResponse;
            }

            Item item = default;
            if (packet.Attachment > 0)
            {
                if (!player.Zone.TryGetGameObject(packet.Attachment, out item))
                {
                    response.Code = MailResponseCode.InvalidAttachment;
                    goto sendResponse;
                }
                
                if (item.IsBound)
                {
                    response.Code = MailResponseCode.ItemCannotBeMailed;
                    goto sendResponse;
                }
                
                await player.GetComponent<InventoryManagerComponent>().RemoveItemAsync(item, packet.AttachmentCount);
            }

            var mail = new CharacterMail
            {
                AuthorId = author.CharacterId,
                RecipientId = recipient.Id,
                AttachmentCurrency = packet.Currency,
                AttachmentLot = item?.Lot ?? -1,
                AttachmentCount = packet.AttachmentCount,
                Subject = packet.Subject,
                Body = packet.Body,
                SentTime = DateTime.Now,
                ExpirationTime = DateTime.Now.AddDays(7)
            };

            await ctx.Mails.AddAsync(mail);
            author.Currency -= 25; // Hard coded cost for filing a mail
            response.Code = MailResponseCode.Success;

            sendResponse:
            await ctx.SaveChangesAsync();
            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.SendResponse,
                MailStruct = response
            });
        }

        public static async Task DataRequestHandler(Player player)
        {
            await using var ctx = new UchuContext();

            var author = player.GetComponent<CharacterComponent>();
            var mails = await ctx.Mails.Where(
                m => m.RecipientId == author.CharacterId
            ).ToArrayAsync();

            var response = new MailData
            {
                Mails = mails
            };
            
            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.Data,
                MailStruct = response
            });
        }

        public static async Task AttachmentCollectHandler(MailAttachmentCollected packet, Player player)
        {
            var response = new AttachmentCollectConfirm
            {
                MailId = packet.MailId
            };
            
            await using var ctx = new UchuContext();
            await using var clientContext = new CdClientContext();

            var mail = await ctx.Mails.FirstOrDefaultAsync(m => m.Id == packet.MailId);
            if (mail == default)
            {
                response.Code = MailAttachmentCollectCode.MailForFound;
                goto sendResponse;
            }

            if (mail.AttachmentLot == -1 || mail.AttachmentCount == default)
            {
                response.Code = MailAttachmentCollectCode.InvalidAttachment;
                
                goto sendResponse;
            }

            await player.GetComponent<InventoryManagerComponent>().AddLotAsync(mail.AttachmentLot, mail.AttachmentCount, lootType: LootType.Mail);

            mail.AttachmentLot = -1;
            mail.AttachmentCount = 0;

            var character = player.GetComponent<CharacterComponent>();
            character.Currency += (long) mail.AttachmentCurrency;
            response.Code = MailAttachmentCollectCode.Success;
            
            sendResponse:

            await ctx.SaveChangesAsync();
            
            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.AttachmentCollectedConfirm,
                MailStruct = response
            });
        }

        public static async Task DeleteHandler(MailDelete packet, Player player)
        {
            var response = new DeleteConfirm
            {
                MailId = packet.MailId
            };
            
            await using var ctx = new UchuContext();
            
            var mail = await ctx.Mails.FirstOrDefaultAsync(m => m.Id == packet.MailId);

            if (mail == default)
            {
                response.Code = MailDeleteCode.MailNotFound;
                
                goto sendResponse;
            }

            response.Code = MailDeleteCode.Success;

            ctx.Mails.Remove(mail);
            
            sendResponse:

            await ctx.SaveChangesAsync();
            
            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.DeleteConfirm,
                MailStruct = response
            });
        }

        public static async Task ReadHandler(MailRead packet, Player player)
        {
            await using var ctx = new UchuContext();
            
            var mail = await ctx.Mails.FirstOrDefaultAsync(m => m.Id == packet.MailId);

            if (mail == default)
            {
                return;
            }

            mail.Read = true;

            await ctx.SaveChangesAsync();
            
            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.ReadConfirm,
                MailStruct = new ReadConfirm
                {
                    MailId = packet.MailId
                }
            });
        }

        public static async Task NotificationRequestHandler(Player player)
        {
            await using var ctx = new UchuContext();

            var response = new Notification();

            var author = player.GetComponent<CharacterComponent>();
            var unreadCount = ctx.Mails.Count(m => m.RecipientId == author.CharacterId && m.Read == false);

            response.MailCountDelta = (uint) unreadCount;

            player.Message(new ServerMailPacket
            {
                Id = ServerMailPacketId.Notification,
                MailStruct = response
            });
        }
    }
}