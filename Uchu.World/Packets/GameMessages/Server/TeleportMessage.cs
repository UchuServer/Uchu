using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class TeleportMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Teleport;

        public bool NoGravity { get; set; }

        public bool IgnoreY { get; set; } = true;

        public bool SetRotation { get; set; }

        public bool SkipAllChecks { get; set; }

        public Vector3 Position { get; set; }

        public bool UseNavMesh { get; set; }

        public Quaternion Rotation { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(IgnoreY);
            writer.WriteBit(SetRotation);
            writer.WriteBit(SkipAllChecks);

            writer.Write(Position);

            writer.WriteBit(UseNavMesh);
            writer.WriteBit(NoGravity);

            writer.Write(Rotation);

        }
    }
}