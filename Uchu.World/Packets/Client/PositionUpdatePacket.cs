using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class PositionUpdatePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;
        public override uint PacketId => 0x16;

        public Vector3 Position { get; set; }
        public Vector4 Rotation { get; set; }
        public bool IsOnGround { get; set; }
        public bool NegativeAngularVelocity { get; set; }
        public Vector3? Velocity { get; set; } = null;
        public Vector3? AngularVelocity { get; set; } = null;
        public long PlatformObjectId { get; set; } = -1;
        public Vector3 PlatformPosition { get; set; }

        public override void Deserialize(BitStream stream)
        {
            Position = new Vector3
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat()
            };
            Rotation = new Vector4
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat(),
                W = stream.ReadFloat()
            };

            IsOnGround = stream.ReadBit();

            NegativeAngularVelocity = stream.ReadBit();

            if (stream.ReadBit())
            {
                Velocity = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };
            }

            if (stream.ReadBit())
            {
                AngularVelocity = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };
            }

            if (stream.ReadBit())
            {
                PlatformObjectId = stream.ReadLong();

                // This is a guess
                PlatformPosition = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };

                if (stream.ReadBit())
                {
                    // Another Vector3?
                    stream.ReadFloat();
                    stream.ReadFloat();
                    stream.ReadFloat();
                }
            }
        }
    }
}