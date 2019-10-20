using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class KnockbackMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.Knockback;
        
        public GameObject Caster { get; set; }
        
        public GameObject Originator { get; set; }
        
        public int KnockbackTime { get; set; }
        
        public Vector3 Vector { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(Caster != default))
                writer.Write(Caster);
            
            if (writer.Flag(Originator != default))
                writer.Write(Originator);
            
            if (writer.Flag(KnockbackTime != default))
                writer.Write(KnockbackTime);

            writer.Write(Vector);
        }
    }
}