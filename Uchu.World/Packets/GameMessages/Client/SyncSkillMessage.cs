using RakDotNet.IO;

namespace Uchu.World
{
    public class SyncSkillMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SyncSkill;

        public bool Done { get; set; }

        public byte[] Content { get; set; }

        public uint BehaviorHandle { get; set; }

        public uint SkillHandle { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Done = reader.ReadBit();
            Content = new byte[reader.Read<uint>()];

            for (var i = 0; i < Content.Length; i++)
                Content[i] = reader.Read<byte>();

            BehaviorHandle = reader.Read<uint>();
            SkillHandle = reader.Read<uint>();
        }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Done);
            writer.Write((uint) Content.Length);
            
            foreach (var b in Content)
                writer.Write(b);

            writer.Write(BehaviorHandle);
            writer.Write(SkillHandle);
        }
    }
}