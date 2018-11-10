using RakDotNet;

namespace Uchu.Core
{
    public class SkillComponent : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
        }

        public override void Construct(BitStream stream)
        {
            stream.WriteBit(false);
        }
    }
}