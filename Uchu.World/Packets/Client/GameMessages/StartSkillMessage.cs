using System;
using System.Numerics;
using System.Runtime.InteropServices;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class StartSkillMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0077;

        public bool IsMouseClick { get; set; }

        public long ConsumableItemId { get; set; }

        public float CasterLatency { get; set; }

        public int CastType { get; set; }

        public Vector3 LastClickPosition { get; set; }

        public long OriginObjectId { get; set; }

        public long TargetObjectId { get; set; }

        public Vector4 OriginRotation { get; set; }

        public byte[] Data { get; set; }

        public uint SkillId { get; set; }

        public uint SkillHandle { get; set; }

        public override void Deserialize(BitStream stream)
        {
            IsMouseClick = stream.ReadBit();
            ConsumableItemId = stream.ReadBit() ? stream.ReadLong() : -1;
            CasterLatency = stream.ReadBit() ? stream.ReadFloat() : 0;
            CastType = stream.ReadBit() ? stream.ReadInt() : 0;
            LastClickPosition = stream.ReadBit() ? new Vector3
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat()
            } : Vector3.Zero;
            OriginObjectId = stream.ReadLong();
            TargetObjectId = stream.ReadBit() ? stream.ReadLong() : -1;
            OriginRotation = stream.ReadBit() ? new Vector4
            {
                X = stream.ReadFloat(),
                Y = stream.ReadFloat(),
                Z = stream.ReadFloat(),
                W = stream.ReadFloat()
            } : Vector4.Zero;
            Data = stream.Read((int) stream.ReadUInt());
            SkillId = stream.ReadUInt();
            SkillHandle = stream.ReadBit() ? stream.ReadUInt() : 0;
        }
    }
}