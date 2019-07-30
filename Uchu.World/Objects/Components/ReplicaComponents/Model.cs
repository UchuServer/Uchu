using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class Model : ReplicaComponent
    {
        // TODO: Look into this
    
        public override ReplicaComponentsId Id => ReplicaComponentsId.Model;
        
        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}