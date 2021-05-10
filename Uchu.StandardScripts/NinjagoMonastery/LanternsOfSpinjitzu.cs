using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class LanternsOfSpinjitzu : NativeScript
    {
        public override Task LoadAsync()
        {
            // Earth lanterns (railposts)
            var gameObjects = Zone.GameObjects.Where(g => g.Lot == 14387);

            // This listens for all earth railposts instead of just the one near Cole but that's probably fine
            foreach (var gameObject in gameObjects)
            {
                Listen(gameObject.OnInteract, async player =>
                {
                    if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                        return;
                    if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                        return;

                    // Cole's mission
                    if (missionInventory.HasActive(2072))
                        await characterComponent.SetFlagAsync(2020, true);
                });
            }

            return Task.CompletedTask;
        }
    }
}