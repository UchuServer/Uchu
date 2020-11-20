using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;

namespace Uchu.StandardScripts.AvantGardens
{
    public class MaelstromSample : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 14718))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject)) return;
                
                if (gameObject.Lot != 14718) return;

                Mount(gameObject);
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
        }
    }
}