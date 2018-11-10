using RakDotNet;

namespace Uchu.Core
{
    public class ScriptedActivityComponent : ReplicaComponent
    {
        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(false); // TODO: implement whole component
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);
        }
    }
}