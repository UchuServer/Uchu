using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Switch : ReplicaComponent
    {
        public bool State { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Switch;
        
        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
            writer.Write(State);
        }
    }
}