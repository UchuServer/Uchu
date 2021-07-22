using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1534_script_name__removed")]
    public class HealthSpawner : ObjectScript
    {
        private const int HealthCount = 3;

        public HealthSpawner(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, player => {
                // Drop health
                for (var i = 0; i < HealthCount; i++)
                {
                    var loot = InstancingUtilities.InstantiateLoot(Lot.ThreeHealth,
                        player, gameObject, gameObject.Transform.Position + Vector3.UnitY * 3);
                    Start(loot);
                }
            });
        }
    }
}
