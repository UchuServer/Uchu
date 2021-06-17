using System.Net.Mail;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class Altars : NativeScript
    {
        private const string ScriptName = "ScriptComponent_1613_script_name__removed";
        private const int ImaginationLot = Lot.ThreeImagination;
        private const int ImaginationCount = 5;

        public override Task LoadAsync()
        {
            foreach (var altar in HasLuaScript(ScriptName))
            {
                Listen(altar.OnInteract, player =>
                {
                    if (altar.GetComponent<QuickBuildComponent>().State != RebuildState.Completed)
                        return;

                    for (var i = 0; i < ImaginationCount; i++)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(ImaginationLot,
                            player, altar, altar.Transform.Position + Vector3.UnitY * 3);

                        Start(loot);
                    }
                });
            }

            return Task.CompletedTask;
        }
    }
}
