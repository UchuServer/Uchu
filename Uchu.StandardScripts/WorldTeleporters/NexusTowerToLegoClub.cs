using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.WorldTeleporters
{
    [ScriptName("ScriptComponent_1485_script_name__removed")]
    public class NexusTowerToLegoClub : BaseWorldTeleporter
    {
        protected override string MessageBoxText => "%[]";
        protected override int TargetZone => 1700;
        protected override string Animation => "lup-teleport";

        public NexusTowerToLegoClub(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
