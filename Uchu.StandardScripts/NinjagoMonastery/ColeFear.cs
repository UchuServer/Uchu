using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class ColeFear : NativeScript
    {
        public override Task LoadAsync()
        {
            var cole = Zone.GameObjects.First(g => g.Lot == 13790);

            Listen(cole.OnEmoteReceived, (emote, player) =>
            {
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    return;
                // Mission to scare Cole
                if (!missionInventory.HasActive(1818))
                    return;
                // Emote: Dragon roar, target: Cole
                missionInventory.ScriptAsync(2581, 13790);
            });

            return Task.CompletedTask;
        }
    }
}