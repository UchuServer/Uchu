using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class OrientToPositionMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x038a;

        public Vector3 Position { get; set; } = Vector3.Zero;
        
        public override void Serialize(BitStream stream)
        {
            stream.WriteFloat(Position.X);
            stream.WriteFloat(Position.Y);
            stream.WriteFloat(Position.Z);
        }
    }
}