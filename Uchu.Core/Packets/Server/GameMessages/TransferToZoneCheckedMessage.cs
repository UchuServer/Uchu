using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class TransferToZoneCheckedMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0205;

        public bool HasQueue { get; set; } = false;
        public uint CloneId { get; set; } = 0;
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Vector4 Rotation { get; set; } = Vector4.Zero;
        public string Spawnpoint { get; set; }
        public byte UcInstanceType { get; set; } = 0;
        public ushort ZoneId { get; set; } = 0;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(HasQueue);

            var hasClone = CloneId != 0;

            stream.WriteBit(hasClone);

            if (hasClone)
                stream.WriteUInt(CloneId);

            var hasPos = Position != Vector3.Zero;

            stream.WriteBit(hasPos);

            if (hasPos)
            {
                stream.WriteFloat(Position.X);
                stream.WriteFloat(Position.Y);
                stream.WriteFloat(Position.Z);
            }

            var hasRot = Rotation != Vector4.Zero;

            stream.WriteBit(hasRot);

            if (hasRot)
            {
                stream.WriteFloat(Rotation.W);
                stream.WriteFloat(Rotation.X);
                stream.WriteFloat(Rotation.Y);
                stream.WriteFloat(Rotation.Z);
            }

            stream.WriteUInt((uint) Spawnpoint.Length);
            stream.WriteString(Spawnpoint, Spawnpoint.Length, true);
            stream.WriteByte(UcInstanceType);

            var hasZone = ZoneId != 0;

            stream.WriteBit(hasZone);

            if (hasZone)
                stream.WriteUShort(ZoneId);
        }
    }
}