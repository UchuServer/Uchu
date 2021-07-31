using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.WorldTeleporters
{
    [ScriptName("ScriptComponent_1484_script_name__removed")]
    [ScriptName("ScriptComponent_1276_script_name__removed")]
    public class ToStarbase3001 : BaseWorldTeleporter
    {
        protected override string MessageBoxText => "%[UI_TRAVEL_TO_LUP_STATION]";
        protected override int TargetZone => 1600;
        protected override string Animation => "lup-teleport";

        public ToStarbase3001(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
