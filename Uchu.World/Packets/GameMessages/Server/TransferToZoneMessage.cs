using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class TransferToZoneMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.TransferToZone;

        public bool CheckTransferAllowed { get; set; }

        public uint CloneId { get; set; }

        public Vector3 Position { get; set; } = Vector3.Zero;

        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        public string SpawnPoint { get; set; }

        public ushort InstanceId { get; set; }

        public ZoneId ZoneId { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(CheckTransferAllowed);

            writer.Write(CloneId);

            writer.WriteBit(true);
            writer.Write(Position.X);
            writer.WriteBit(true);
            writer.Write(Position.Y);
            writer.WriteBit(true);
            writer.Write(Position.Z);

            writer.WriteBit(true);
            writer.Write(Rotation.W);
            writer.WriteBit(true);
            writer.Write(Rotation.X);
            writer.WriteBit(true);
            writer.Write(Rotation.Y);
            writer.WriteBit(true);
            writer.Write(Rotation.Z);

            writer.Write((uint) SpawnPoint.Length);
            writer.WriteString(SpawnPoint, SpawnPoint.Length, true);

            writer.Write(InstanceId);
            writer.Write((int) ZoneId);
        }
    }
}