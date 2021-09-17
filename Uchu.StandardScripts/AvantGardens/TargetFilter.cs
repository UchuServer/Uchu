using System.Linq;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Script to show/hide targets based on whether the player has the relevant mission
    /// </summary>
    [ZoneSpecific(1100)]
    public class TargetFilter : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 14380))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, @object =>
            {
                if (@object is GameObject gameObject && gameObject.Lot == 14380)
                {
                    Mount(gameObject);
                }
            });

            return Task.CompletedTask;
        }

        public static void Mount(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<MissionFilterComponent>(out var missionFilter))
            {
                missionFilter = gameObject.AddComponent<MissionFilterComponent>();
            }
            missionFilter.AddMissionIdToFilter(MissionId.SixShooter);
        }
    }
}
