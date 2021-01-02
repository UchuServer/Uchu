using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ZoneSpecific(1100)]
    public class MaelstromVacuum : NativeScript
    {
        public override Task LoadAsync()
        {
            Listen(Zone.OnObject, async obj =>
            {
                if (!(obj is AuthoredGameObject gameObject)) return;
                
                if (gameObject.Lot != 14596) return;

                var samples = Zone.GameObjects.Where(g => g.Lot == 14718).ToList();

                var position = gameObject.Transform.Position;

                samples.Sort((a, b) => (int) (
                    Vector3.Distance(position, a.Transform.Position) -
                    Vector3.Distance(position, b.Transform.Position)
                ));

                var selected = samples.FirstOrDefault();
                
                if (selected == default) return;

                if (Vector3.Distance(position, selected.Transform.Position) > 15) return;

                var smashable = selected.GetComponent<DestructibleComponent>();

                await smashable.SmashAsync(gameObject, gameObject.Author as Player);
            });
            
            return Task.CompletedTask;
        }
    }
}