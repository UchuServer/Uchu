using RakDotNet.IO;

namespace Uchu.World
{
    public class SwitchComponent : ReplicaComponent
    {
        public bool State { get; set; }

        public override ComponentId Id => ComponentId.SwitchComponent;

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(State);
        }
    }
}