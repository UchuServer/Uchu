using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("ScriptComponent_815_script_name__removed")]
    public class TurretMech : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TurretMech(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) return;

            // Listen for the mech being destroyed.
            Listen(destructibleComponent.OnSmashed, (smasher, lootOwner) =>
            {
                // Create the turret quick build.
                var quickBuild = GameObject.Instantiate<AuthoredGameObject>(
                    Zone,
                    6254,
                    gameObject.Transform.Position,
                    gameObject.Transform.Rotation,
                    smasher
                );
                Start(quickBuild);
                Construct(quickBuild);

                // Destroy the turret after 20 seconds.
                Task.Run(async () =>
                {
                    await Task.Delay(20000);
                    Zone.BroadcastMessage(new DieMessage
                    {
                        Associate = quickBuild,
                        DeathType = "",
                        Killer = smasher,
                        SpawnLoot = false,
                        LootOwner = quickBuild
                    });
                    Destroy(quickBuild);
                });
            });
        }
    }
}