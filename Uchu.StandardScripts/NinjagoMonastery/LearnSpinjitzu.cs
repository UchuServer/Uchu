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
                    if (instance.MissionId == 1796) // Cole mission (Earth)
                        characterComponent.SetFlagAsync(2030, true);
                    // source: https://lu.lcdruniverse.org/explorer/missions/2053

                    if (instance.MissionId == 1952) // Jay mission (Lightning)
                        characterComponent.SetFlagAsync(2031, true);
                    // source: https://lu.lcdruniverse.org/explorer/missions/2054

                    if (instance.MissionId == 1959) // Zane mission (Ice)
                        characterComponent.SetFlagAsync(2032, true);
                    // source: https://lu.lcdruniverse.org/explorer/missions/2055

                    if (instance.MissionId == 1962) // Kai mission (Fire)
                        characterComponent.SetFlagAsync(2033, true);
                    // source: https://lu.lcdruniverse.org/explorer/missions/2056
                });
            });

            return Task.CompletedTask;
        }
    }
}