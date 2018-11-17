using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class BuildModeSetMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x042D;

        public bool Start { get; set; }
        public int DistanceType { get; set; } = -1;
        public bool IsModePaused { get; set; }
        public int ModeValue { get; set; } = 1;
        public long PlayerObjectId { get; set; }
        public Vector3 StartPosition { get; set; } = Vector3.Zero;

        public override void Deserialize(BitStream stream)
        {
            Start = stream.ReadBit();

            if (stream.ReadBit())
                DistanceType = stream.ReadInt();

            IsModePaused = stream.ReadBit();

            if (stream.ReadBit())
                ModeValue = stream.ReadInt();

            PlayerObjectId = stream.ReadLong();

            if (stream.ReadBit())
                StartPosition = new Vector3
                {
                    X = stream.ReadFloat(),
                    Y = stream.ReadFloat(),
                    Z = stream.ReadFloat()
                };
        }
    }
}