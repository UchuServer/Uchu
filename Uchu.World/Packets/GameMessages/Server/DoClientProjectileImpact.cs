using RakDotNet.IO;

namespace Uchu.World
{
    public class DoClientProjectileImpact : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DoClientProjectileImpact;
        
        public long ProjectileId { get; set; }
        
        public GameObject Owner { get; set; }
        
        public GameObject Target { get; set; }
        
        public byte[] Data { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            if (writer.Flag(ProjectileId != default))
                writer.Write(ProjectileId);

            if (writer.Flag(Owner != default))
                writer.Write(Owner);

            if (writer.Flag(Target != default))
                writer.Write(Target);

            writer.Write((uint) Data.Length);

            foreach (var b in Data)
            {
                writer.Write(b);
            }
        }
    }
}