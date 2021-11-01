using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    /// Script for spawned maelstrom vacuum (https://lu.lcdruniverse.org/explorer/objects/14596)
    /// </summary>
    [ScriptName("ScriptComponent_1582_script_name__removed")]
    public class MaelstromVacuum : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public MaelstromVacuum(GameObject gameObject) : base(gameObject)
        {
            if (!(gameObject is AuthoredGameObject authoredGameObject)) return;
            if (!(authoredGameObject.Author is Player player)) return;
            
            // Listen to the player attempting to collect.
            Listen(player.OnFireServerEvent, (name, message) =>
            {
                if (name != "attemptCollection") return;
                if (message.Associate != gameObject) return;

                // Determine the closest sample and return if it is too far.
                var samples = Zone.GameObjects.Where(g => g.Lot == 14718).ToList();
                var position = gameObject.Transform.Position;
                samples.Sort((a, b) => (int) (
                    Vector3.Distance(position, a.Transform.Position) -
                    Vector3.Distance(position, b.Transform.Position)
                ));
                var selected = samples.FirstOrDefault();
                if (selected == default) return;
                if (Vector3.Distance(position, selected.Transform.Position) > 15) return;

                // Animate the vacuum and clear it.
                var smashable = selected.GetComponent<DestructibleComponent>();
                Task.Run(async () =>
                {
                    gameObject.Animate("collect_maelstrom");
                    await Task.Delay(4000);
                    await smashable.SmashAsync(gameObject, authoredGameObject.Author as Player);
                });
            });
            
            // List to the player being ready for updates (animations).
            Listen(player.OnReadyForUpdatesEvent, (message) =>
            {
                if (message.GameObject.Lot != 14596) return;
                if (message.GameObject != gameObject) return;
                this.PlayAnimation("idle_maelstrom");
            });
        }
    }
}
