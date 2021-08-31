using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1535_script_name__removed")]
    public class ArmorSpawner : ObjectScript
    {
        private const int ArmorCount = 3;

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ArmorSpawner(GameObject gameObject) : base(gameObject)
        {
            Listen(gameObject.OnInteract, player => {
                // Drop armor
                for (var i = 0; i < ArmorCount; i++)
                {
                    var loot = InstancingUtilities.InstantiateLoot(Lot.ThreeArmor,
                        player, gameObject, gameObject.Transform.Position + Vector3.UnitY * 3);
                    Start(loot);
                }

                // Terminate interaction so the player can interact again.
                player.Message(new TerminateInteractionMessage
                {
                    Associate = player,
                    Terminator = gameObject,
                    Type = TerminateType.FromInteraction,
                });
            });
        }
    }
}
