using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.RacingControl;

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);
            
            // TODO: Look into to
        }
    }
}