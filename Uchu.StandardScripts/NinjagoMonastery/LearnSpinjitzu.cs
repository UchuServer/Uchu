using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class LearnSpinjitzu : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnPlayerLoad, player =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                    return;

                Listen(missionInventory.OnCompleteMission, instance =>
                {
                    switch (instance.MissionId)
                    {
                        // Cole mission (Earth)
                        case 1796:
                            characterComponent.SetFlagAsync(2030, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2053
                            break;
                        // Jay mission (Lightning)
                        case 1952:
                            characterComponent.SetFlagAsync(2031, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2054
                            break;
                        // Zane mission (Ice)
                        case 1959:
                            characterComponent.SetFlagAsync(2032, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2055
                            break;
                        // Kai mission (Fire)
                        case 1962:
                            characterComponent.SetFlagAsync(2033, true);
                            // source: https://lu.lcdruniverse.org/explorer/missions/2056
                            break;
                    }
                });
            });

            return Task.CompletedTask;
        }
    }
}