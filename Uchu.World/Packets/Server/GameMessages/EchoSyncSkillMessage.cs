using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class EchoSyncSkillMessage : ServerGameMessage
    {
        public override ushort GameMessageId => 0x0478;

        public bool IsDone { get; set; } = false;

        public byte[] Data { get; set; }

        public uint BehaviorHandle { get; set; }

        public uint SkillHandle { get; set; }

        public override void SerializeMessage(BitStream stream)
        {
            stream.WriteBit(IsDone);
            stream.WriteUInt((uint) Data.Length);
            stream.Write(Data);
            stream.WriteUInt(BehaviorHandle);
            stream.WriteUInt(SkillHandle);
        }
    }
}