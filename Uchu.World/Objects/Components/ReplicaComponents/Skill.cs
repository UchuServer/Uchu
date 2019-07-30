using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Skill : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Skill;
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}