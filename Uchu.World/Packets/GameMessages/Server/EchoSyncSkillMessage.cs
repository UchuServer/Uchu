using RakDotNet.IO;

namespace Uchu.World
{
    public class EchoSyncSkillMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.EchoSyncSkill;

        public bool Done { get; set; }

        public byte[] Content { get; set; }

        public uint BehaviorHandle { get; set; }

        public uint SkillHandle { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Done);

            writer.Write((uint) Content.Length);
            foreach (var b in Content) writer.Write(b);

            writer.Write(BehaviorHandle);
            writer.Write(SkillHandle);
        }
    }
}