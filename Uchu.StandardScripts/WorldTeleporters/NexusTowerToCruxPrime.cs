using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.WorldTeleporters
{
    [ScriptName("ScriptComponent_1527_script_name__removed")]
    public class NexusTowerToCruxPrime : BaseWorldTeleporter
    {
        protected override string MessageBoxText => "%[UI_TRAVEL_TO_CRUX_PRIME]";
        protected override int TargetZone => 1800;
        protected override string Animation => "nexus-teleport";

        public NexusTowerToCruxPrime(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
