using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class SkillComponent : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.Skill;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}