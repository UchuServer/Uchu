using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    public class TargetFilter : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 14718 || g.Lot == 14380))
            {
                Mount(gameObject);
            }

            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject)) return;
                
                if (gameObject.Lot != 14718 && gameObject.Lot != 14380) return;

                Mount(gameObject);
            });
            
            return Task.CompletedTask;
        }

        public static void Mount(GameObject gameObject)
        {
            gameObject.AddComponent<MissionFilterComponent>().AddMissionIdToFiler(1880);
        }
    }
}