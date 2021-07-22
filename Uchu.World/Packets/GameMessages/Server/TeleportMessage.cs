using System;
using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class TeleportMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Teleport;

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

            writer.WriteBit(Math.Abs(Rotation.W) > 0.01f);
            writer.Write(Rotation.W);
            writer.Write(Rotation.X);
            writer.Write(Rotation.Y);
            writer.Write(Rotation.Z);

        }
    }
}
