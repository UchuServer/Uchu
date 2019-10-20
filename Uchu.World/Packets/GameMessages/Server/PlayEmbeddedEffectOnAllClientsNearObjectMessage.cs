using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class PlayEmbeddedEffectOnAllClientsNearObjectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayEmbeddedEffectOnAllClientsNearObject;
        
        public string EffectName { get; set; }
        
        public GameObject FromObject { get; set; }
        
        public float Radius { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) EffectName.Length);
            writer.WriteString(EffectName, EffectName.Length, true);

            writer.Write(FromObject);

            writer.Write(Radius);
        }
    }
}