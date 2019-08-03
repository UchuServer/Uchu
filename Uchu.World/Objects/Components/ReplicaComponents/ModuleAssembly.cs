using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class ModuleAssembly : ReplicaComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.ModuleAssembly;
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}