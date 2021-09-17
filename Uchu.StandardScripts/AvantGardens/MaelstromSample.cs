using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Script to show/hide maelstrom samples based on whether the player has the relevant mission
    /// </summary>
    [ZoneSpecific(1100)]
    public class MaelstromSample : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 14718))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, @object =>
            {
                if (@object is GameObject gameObject && gameObject.Lot == 14718)
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
            missionFilter.AddMissionIdToFilter(MissionId.FollowingtheTrail);
            missionFilter.AddMissionIdToFilter(MissionId.SampleforScience);
        }
    }
}
