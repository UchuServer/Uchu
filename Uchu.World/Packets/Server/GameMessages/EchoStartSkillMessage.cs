using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class EchoStartSkillMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0076;

        public bool IsMouseClick { get; set; } = false;

        public float CasterLatency { get; set; } = 0;

        public int CastType { get; set; } = 0;

        public Vector3 LastClickPosition { get; set; } = Vector3.Zero;

        public long OriginObjectId { get; set; }

        public long TargetObjectId { get; set; } = -1;

        public Vector4 OriginRotation { get; set; } = Vector4.Zero;

        public byte[] Data { get; set; }

        public uint SkillId { get; set; }

        public uint SkillHandle { get; set; } = 0;

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(IsMouseClick);

            var hasLatency = !CasterLatency.Equals(0);

            stream.WriteBit(hasLatency);

            if (hasLatency)
                stream.WriteFloat(CasterLatency);

            var hasType = CastType != 0;

            stream.WriteBit(hasType);

            if (hasType)
                stream.WriteInt(CastType);

            var hasClickPos = LastClickPosition != Vector3.Zero;

            stream.WriteBit(hasClickPos);

            if (hasClickPos)
            {
                stream.WriteFloat(LastClickPosition.X);
                stream.WriteFloat(LastClickPosition.Y);
                stream.WriteFloat(LastClickPosition.Z);
            }

            stream.WriteLong(OriginObjectId);

            var hasTarget = TargetObjectId != -1;

            stream.WriteBit(hasTarget);

            if (hasTarget)
                stream.WriteLong(TargetObjectId);

            var hasRot = OriginRotation != Vector4.Zero;

            stream.WriteBit(hasRot);

            if (hasRot)
            {
                stream.WriteFloat(OriginRotation.X);
                stream.WriteFloat(OriginRotation.Y);
                stream.WriteFloat(OriginRotation.Z);
                stream.WriteFloat(OriginRotation.W);
            }

            stream.WriteUInt((uint) Data.Length);
            stream.Write(Data);

            stream.WriteUInt(SkillId);

            var hasHandle = SkillHandle != 0;

            stream.WriteBit(hasHandle);

            if (hasHandle)
                stream.WriteUInt(SkillHandle);
        }
    }
}