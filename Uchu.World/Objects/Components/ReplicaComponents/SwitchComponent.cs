using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [Essential]
    public class SwitchComponent : ReplicaComponent
    {
        public bool State { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Switch;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(State);
        }
    }
}