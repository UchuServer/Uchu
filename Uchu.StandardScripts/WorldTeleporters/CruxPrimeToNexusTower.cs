using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.WorldTeleporters
{
    [ScriptName("ScriptComponent_1486_script_name__removed")]
    public class CruxPrimeToNexusTower : BaseWorldTeleporter
    {
        protected override string MessageBoxText => "%[UI_TRAVEL_TO_NEXUS_TOWER]";
        protected override int TargetZone => 1900;
        protected override string TargetSpawnLocation => "cp_door";
        protected override string Animation => "nexus-teleport";

        public CruxPrimeToNexusTower(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
