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
            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject)) return;

                MountTurret(gameObject);
            });

            foreach (var gameObject in Zone.GameObjects)
            {
                MountTurret(gameObject);
            }
            
            return Task.CompletedTask;
        }

        public void MountTurret(GameObject gameObject)
        {
            if (gameObject.Lot != 6253) return;

            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) return;

            Listen(destructibleComponent.OnSmashed, (smasher, lootOwner) =>
            {
                var quickBuild = GameObject.Instantiate<AuthoredGameObject>(
                    Zone,
                    6254,
                    gameObject.Transform.Position,
                    gameObject.Transform.Rotation
                );

                quickBuild.Author = smasher;

                Start(quickBuild);
                Construct(quickBuild);

                Task.Run(async () =>
                {
                    await Task.Delay(20000);

                    await quickBuild.GetComponent<DestructibleComponent>().SmashAsync(quickBuild, lootOwner);

                    Destroy(quickBuild);
                });
            });
        }
    }
}