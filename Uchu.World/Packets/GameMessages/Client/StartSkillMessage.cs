using System;
using System.Linq;
using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class StartSkillMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartSkill;

        public bool UsedMouse { get; set; }

        public Item ConsumableItem { get; set; }

        public float CasterLatency { get; set; }

        public int CastType { get; set; }

        public Vector3 LastClickedPosition { get; set; }

        public GameObject OptionalOriginator { get; set; }

        public GameObject OptionalTarget { get; set; }

        public Quaternion OriginatorRotation { get; set; } = Quaternion.Identity;

        public byte[] Content { get; set; }

        public int SkillId { get; set; }

        public uint SkillHandle { get; set; }

        public override void Deserialize(BitReader reader)
        {
            UsedMouse = reader.ReadBit();

            if (reader.ReadBit())
                ConsumableItem = reader.ReadGameObject<Item>(Associate.Zone);

            if (reader.ReadBit())
                CasterLatency = reader.Read<float>();

            if (reader.ReadBit())
                CastType = reader.Read<int>();

            if (reader.ReadBit())
                LastClickedPosition = reader.Read<Vector3>();

            OptionalOriginator = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit())
                OptionalTarget = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit())
                OriginatorRotation = reader.Read<Quaternion>();

            Content = reader.ReadBytes((int) reader.Read<uint>()).ToArray();

            SkillId = reader.Read<int>();

            if (reader.ReadBit())
                SkillHandle = reader.Read<uint>();
        }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(UsedMouse);

            if (writer.Flag(ConsumableItem != default))
                writer.Write(ConsumableItem);

            if (writer.Flag(!CasterLatency.Equals(0)))
                writer.Write(CasterLatency);

            if (writer.Flag(CastType != default))
                writer.Write(CastType);

            if (writer.Flag(LastClickedPosition != Vector3.Zero))
                writer.Write(LastClickedPosition);

            writer.Write(OptionalOriginator);

            if (writer.Flag(OptionalTarget != default))
                writer.Write(OptionalTarget);

            if (writer.Flag(OriginatorRotation != Quaternion.Identity))
                writer.Write(OriginatorRotation);

            writer.Write((uint) Content.Length);

            foreach (var b in Content)
            {
                writer.Write(b);
            }

            writer.Write(SkillId);

            if (writer.Flag(SkillHandle != 0))
                writer.Write(SkillHandle);
        }
    }
}