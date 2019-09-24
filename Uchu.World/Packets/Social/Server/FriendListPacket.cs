using System;
using System.IO;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World.Social
{
    public class FriendListPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x1E;

        public Friend[] Friends { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.Write<byte>(0);

            using (var stream = new MemoryStream())
            using (var friendWriter = new BitWriter(stream))
            {
                friendWriter.Write((ushort) Friends.Length);
                foreach (var friend in Friends) friendWriter.Write(friend);

                var friends = stream.ToArray();

                writer.Write((ushort) (friends.Length - 1));
                writer.Write(friends);
            }
        }

        public class Friend : ISerializable
        {
            public bool IsOnline { get; set; }

            public bool IsBestFriend { get; set; }

            public bool IsFreeToPlay { get; set; }

            public ZoneId ZoneId { get; set; }

            public ushort WorldInstance { get; set; }

            public uint WorldClone { get; set; }

            public long PlayerId { get; set; }

            public string PlayerName { get; set; }

            public void Serialize(BitWriter writer)
            {
                writer.Write((byte) (IsOnline ? 1 : 0));
                writer.Write((byte) (IsBestFriend ? 1 : 0));
                writer.Write((byte) (IsFreeToPlay ? 1 : 0));

                writer.Write(new byte[5]);

                writer.Write((ushort) ZoneId);
                writer.Write(WorldInstance);
                writer.Write(WorldClone);

                writer.Write(PlayerId);
                writer.WriteString(PlayerName, wide: true);

                writer.Write(new byte[6]);
            }

            public void Deserialize(BitReader reader)
            {
            }
        }
    }
}