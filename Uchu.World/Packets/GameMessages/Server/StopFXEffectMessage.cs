using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class StopFXEffectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StopFXEffect;
        
        public bool KillImmediate { get; set; }
        
        public string Name { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(KillImmediate);

            writer.Write((uint) Name.Length);
            writer.WriteString(Name, Name.Length);
        }
    }
}