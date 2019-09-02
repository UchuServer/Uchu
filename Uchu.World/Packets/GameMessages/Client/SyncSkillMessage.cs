using RakDotNet.IO;

namespace Uchu.World
{
    public class SyncSkillMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x479;
        
        public bool Done { get; set; }
        
        public byte[] Content { get; set; }
        
        public uint BehaviourHandle { get; set; }
        
        public uint SkillHandle { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Done = reader.ReadBit();
            
            Content = new byte[reader.Read<uint>()];
            
            for (var i = 0; i < Content.Length; i++)
            {
                Content[i] = reader.Read<byte>();
            }

            BehaviourHandle = reader.Read<uint>();
            SkillHandle = reader.Read<uint>();
        }
    }
}