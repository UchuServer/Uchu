using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.GnarledForest
{
    [ScriptName("l_trigger_ambush.lua")]
    public class Ambush : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Ambush(GameObject gameObject) : base(gameObject)
        {
            var triggered = false;
            var spawner = GetSpawnerByName("Ambush");
            if (gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
            {
                Listen(physicsComponent.OnEnter, collider =>
                {
                    if (collider.GameObject is Player player)
                    {
                        if (!triggered)
                        {
                            //this appears to trigger properly but does nothing for some reason
                            triggered = true;
                            spawner.Activate();
                            Zone.Schedule(() =>
                            {
                                triggered = false;
                                spawner.Reset();
                                spawner.Deactivate();
                            }, 45000);
                        }
                    }
                });
            }
        }
    }
}