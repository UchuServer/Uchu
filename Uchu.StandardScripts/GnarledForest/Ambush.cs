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
            var spawner = GetSpawnerByName("Ambush");
            if (gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
            {
                Listen(physicsComponent.OnEnter, collider =>
                {
                    if (collider.GameObject is not Player) return;
                    if (this.GetVar<bool>("triggered")) return;
                    
                    this.SetVar("triggered", true);
                    spawner.Activate();
                    spawner.SpawnAll();
                    this.AddTimerWithCancel(45, "TriggeredTimer");
                });
            }
        }

        /// <summary>
        /// Callback for the timer completing.
        /// </summary>
        /// <param name="timerName">Timer that was completed.</param>
        public override void OnTimerDone(string timerName)
        {
            this.SetVar("triggered", false);
            var spawner = GetSpawnerByName("Ambush");
            spawner.Deactivate();
        }
    }
}