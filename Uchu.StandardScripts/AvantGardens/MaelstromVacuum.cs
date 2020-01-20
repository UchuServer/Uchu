using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    public class MaelstromVacuum : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is AuthoredGameObject gameObject)) return;
                
                if (gameObject.Lot != 14596) return;

                foreach (var brick in Zone.GameObjects.Where(g => g.Lot == 14718))
                {
                    if (Vector3.Distance(gameObject.Transform.Position, brick.Transform.Position) > 12) continue;

                    brick.GetComponent<DestructibleComponent>().Smash(gameObject.Author);
                }
            });

            return Task.CompletedTask;
        }
    }
}