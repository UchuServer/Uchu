using RakDotNet.IO;

namespace Uchu.World
{
    public class RacingControlComponent : ScriptedActivityComponent
    {
        public override ComponentId Id => ComponentId.RacingControl;

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