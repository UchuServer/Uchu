using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PositionUpdatePacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Client;

        public override uint PacketId => 0x16;

        public Vector3 Position { get; set; }
        
        public Quaternion Rotation { get; set; }
        
        public bool IsOnGround { get; set; }
        
        public bool NegativeAngularVelocity { get; set; }
        
        public bool HasVelocity { get; set; }
        
        public Vector3 Velocity { get; set; }
        
        public bool HasAngularVelocity { get; set; }

        public Vector3 AngularVelocity { get; set; }
        
        public long PlatformObjectId { get; set; } = -1;
        
        public Vector3 PlatformPosition { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Position = reader.Read<Vector3>();
            Rotation = reader.Read<Quaternion>();
            
            IsOnGround = reader.ReadBit();

            NegativeAngularVelocity = reader.ReadBit();

            HasVelocity = reader.ReadBit();
            
            if (HasVelocity)
            {
                Velocity = reader.Read<Vector3>();
            }

            HasAngularVelocity = reader.ReadBit();

            if (HasAngularVelocity)
            {
                AngularVelocity = reader.Read<Vector3>();
            }

            if (!reader.ReadBit()) return;
            
            PlatformObjectId = reader.Read<long>();

            PlatformPosition = reader.Read<Vector3>();

            if (!reader.ReadBit()) return;
            reader.Read<Vector3>();
        }
    }
}