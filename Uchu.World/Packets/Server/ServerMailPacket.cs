using System;
using System.Linq;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ServerMailPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x31;
        
        public ServerMailPacketId Id { get; set; }
        
        public ISerializable MailStruct { get; set; }
        
        public override void SerializePacket(BitWriter writer)
        {
            writer.Write((int) Id);

            MailStruct.Serialize(writer);
        }
        
        public enum ServerMailPacketId
        {
            SendResponse = 0x1,
            Notification = 0x2,
            Data = 0x4,
            AttachmentCollectedConfirm = 0x6,
            DeleteConfirm = 0x8,
            ReadConfirm = 0xA
        }
        
        public enum MailResponseCode
        {
            Success,
            NotEnoughCurrency,
            InvalidAttachment,
            ItemCannotBeMailed,
            CannotMailYourself,
            RecipientNotFound,
            DifferentFaction,
            UnknownFailure,
            ModerationFailure,
            Muted
        }
        
        public enum MailAttachmentCollectCode
        {
            Success,
            InvalidAttachment,
            NotEnoughSpace,
            MailForFound,
            Throttled,
            UnknownFail
        }
        
        public enum MailDeleteCode
        {
            Success,
            InvalidAttachment,
            MailNotFound,
            Throttled,
            UnknownFail
        }
        
        public class SendResponse : ISerializable
        {
            public MailResponseCode Code { get; set; }
            
            public void Serialize(BitWriter writer)
            {
                writer.Write((int) Code);
            }
        }

        public class Notification : ISerializable
        {
            public string UnknownString { get; set; }
            
            public uint MailCountDelta { get; set; }
            
            public uint UnknownInt { get; set; }
            
            public void Serialize(BitWriter writer)
            {
                writer.Write<uint>(0);

                writer.WriteString(UnknownString, 32);

                writer.Write(MailCountDelta);

                writer.Write(UnknownInt);
            }
        }
        
        public class MailData : ISerializable
        {
            public CharacterMail[] Mails { get; set; }

            public void Serialize(BitWriter writer)
            {
                using var ctx = new UchuContext();
                
                writer.Write<uint>(0);
                
                writer.Write((ushort) Mails.Length);
                
                writer.Write<ushort>(0);

                foreach (var mail in Mails)
                {
                    
                    writer.Write(mail.Id);

                    writer.WriteString(mail.Subject, 50, true);
                    writer.WriteString(mail.Body, 400, true);

                    string authorName;

                    if (mail.AuthorId == 0)
                    {
                        authorName = "LEGO Universe";
                    }
                    else
                    {
                        var author = ctx.Characters.FirstOrDefault(c => c.Id == mail.AuthorId);
                        authorName = author?.Name ?? "Deleted Character";
                    }

                    writer.WriteString(authorName ?? "LEGO Universe", 32, true);

                    writer.Write<uint>(0);

                    writer.Write(mail.AttachmentCurrency);

                    writer.Write(mail.AttachmentLot > 0 ? ObjectId.Standalone : ObjectId.Invalid); // Unnecessary

                    writer.Write(mail.AttachmentLot);

                    writer.Write<uint>(0);

                    writer.Write<long>(0);

                    writer.Write(mail.AttachmentCount);

                    for (var i = 0; i < 6; i++)
                    {
                        writer.Write<byte>(0);
                    }

                    writer.Write((ulong) ((DateTimeOffset) mail.ExpirationTime).ToUnixTimeSeconds());

                    writer.Write((ulong) ((DateTimeOffset) mail.SentTime).ToUnixTimeSeconds());

                    writer.Write((byte) (mail.Read ? 1 : 0));

                    writer.Write<byte>(0);

                    writer.Write<ushort>(0);

                    writer.Write<uint>(0);
                }
            }
        }
        
        public class AttachmentCollectConfirm : ISerializable
        {
            public MailAttachmentCollectCode Code { get; set; }
            
            public long MailId { get; set; }
            
            public void Serialize(BitWriter writer)
            {
                writer.Write((uint) Code);

                writer.Write(MailId);
            }
        }
        
        public class DeleteConfirm : ISerializable
        {
            public MailDeleteCode Code { get; set; }
            
            public long MailId { get; set; }
            
            public void Serialize(BitWriter writer)
            {
                writer.Write((uint) Code);

                writer.Write(MailId);
            }
        }
        
        public class ReadConfirm : ISerializable
        {
            public long MailId { get; set; }
            
            public void Serialize(BitWriter writer)
            {
                writer.Write<uint>(1);

                writer.Write(MailId);
            }
        }
    }
}