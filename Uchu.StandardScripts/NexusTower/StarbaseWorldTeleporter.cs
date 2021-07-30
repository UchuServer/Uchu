using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1484_script_name__removed")]
    public class StarbaseWorldTeleporter : BaseWorldTeleporter
    {
        protected override string MessageBoxText => "%[UI_TRAVEL_TO_LUP_STATION]";
        protected override int TargetZone => 1600;
        protected override string Animation => "lup-teleport";

        public StarbaseWorldTeleporter(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
