using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    public class TurretMech : NativeScript
    {
        public override Task LoadAsync()
        {
            foreach (var gameObject in Zone.GameObjects.Where(g => g.Lot == 6253))
            {
                if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) continue;

                Listen(destructibleComponent.OnSmashed, (smasher, lootOwner) =>
                {
                    var quickBuild = GameObject.Instantiate(
                        Zone,
                        6254,
                        gameObject.Transform.Position,
                        gameObject.Transform.Rotation
                    );

                    Start(quickBuild);
                    Construct(quickBuild);

                    Task.Run(async () =>
                    {
                        await Task.Delay(10000); // TODO: Find real number

                        Destroy(quickBuild);
                    });
                });
            }
            
            return Task.CompletedTask;
        }
    }
}