using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ClientMailPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x17;
        
        public ClientMailPacketId Id { get; set; }
        
        public IDeserializable MailStruct { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Id = (ClientMailPacketId) reader.Read<uint>();

            switch (Id)
            {
                case ClientMailPacketId.Send:
                    MailStruct = new MailSend();
                    break;
                case ClientMailPacketId.DataRequest:
                    break;
                case ClientMailPacketId.AttachmentCollected:
                    MailStruct = new MailAttachmentCollected();
                    break;
                case ClientMailPacketId.Delete:
                    MailStruct = new MailDelete();
                    break;
                case ClientMailPacketId.Read:
                    MailStruct = new MailRead();
                    break;
                case ClientMailPacketId.NotificationRequest:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(Id));
            }

            MailStruct?.Deserialize(reader);
        }
        
        public enum ClientMailPacketId
        {
            Send,
            DataRequest = 0x3,
            AttachmentCollected = 0x5,
            Delete = 0x7,
            Read = 0x9,
            NotificationRequest = 0xB
        }
        
        public class MailSend : IDeserializable
        {
            public string Subject { get; set; }
            
            public string Body { get; set; }
            
            public string RecipientName { get; set; }
            
            public ulong Currency { get; set; }
            
            public long Attachment { get; set; }
            
            public ushort AttachmentCount { get; set; }
            
            public ushort LanguageId { get; set; }
            
            public uint UnknownInt { get; set; }
            
            public void Deserialize(BitReader reader)
            {
                Subject = reader.ReadString(50, true);

                Body = reader.ReadString(400, true);

                RecipientName = reader.ReadString(32, true);

                Currency = reader.Read<ulong>();

                Attachment = reader.Read<long>();

                AttachmentCount = reader.Read<ushort>();

                LanguageId = reader.Read<ushort>();

                UnknownInt = reader.Read<uint>();
            }
        }
        
        public class MailAttachmentCollected : IDeserializable
        {
            public uint Unknown { get; set; }
            
            public long MailId { get; set; }
            
            public long Player { get; set; }

            public void Deserialize(BitReader reader)
            {
                Unknown = reader.Read<uint>();

                MailId = reader.Read<long>();

                Player = reader.Read<long>();
            }
        }
        
        public class MailDelete : IDeserializable
        {
            public uint Unknown { get; set; }
            
            public long MailId { get; set; }
            
            public long Player { get; set; }
            
            public void Deserialize(BitReader reader)
            {
                Unknown = reader.Read<uint>();

                MailId = reader.Read<long>();

                Player = reader.Read<long>();
            }
        }
        
        public class MailRead : IDeserializable
        {
            public uint Unknown { get; set; }
            
            public long MailId { get; set; }
            
            public void Deserialize(BitReader reader)
            {
                Unknown = reader.Read<uint>();

                MailId = reader.Read<long>();
            }
        }
    }
}