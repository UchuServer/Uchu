using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("L_NT_PARADOXTELE_SERVER.lua")]
    public class ParadoxTeleporter : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ParadoxTeleporter(GameObject gameObject) : base(gameObject)
        {
            // Get info from object settings
            var group = (string) gameObject.Settings["teleGroup"];
            var cinematic = (string) gameObject.Settings["Cinematic"];

            // All these must have a physics component to detect the player entering.
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var phys))
                return;

            Listen(phys.OnEnter, other =>
            {
                // Should only teleport players.
                if (!(other.GameObject is Player player))
                    return;

                // Find teleporter target location.
                var target = Zone.GameObjects.FirstOrDefault(obj => obj.Settings.TryGetValue("groupID", out var group2)
                                                       && ((string) group2).Split(";").Contains(group));
                if (target == null)
                    return;

                // The player shouldn't be able to move during the animation and the teleportation.
                player.Message(new SetStunnedMessage
                {
                    Associate = player,
                    Originator = gameObject,
                    StateChangeType = StunState.Push,
                    CantAttack = true,
                    CantJump = true,
                    CantMove = true,
                    CantTurn = true,
                });

                // Show teleport animation.
                player.Animate("paradoxdeath");
                // paradox-teleport-in and nexusteleport also exist, don't know if they're used somewhere else.

                // At the end of the cinematic, rebuild the player at the target location.
                Task.Run(async () =>
                {
                    // Wait for the animation to complete.
                    await Task.Delay(2000);

                    // Teleport player to target location.
                    player.Teleport(target.Transform.Position);

                    // Show camera path.
                    player.Message(new PlayCinematicMessage
                    {
                        Associate = player,
                        LockPlayer = true,
                        PathName = cinematic,
                    });

                    // Un-stun player.
                    player.Message(new SetStunnedMessage
                    {
                        Associate = player,
                        Originator = gameObject,
                        StateChangeType = StunState.Pop,
                        CantAttack = true,
                        CantJump = true,
                        CantMove = true,
                        CantTurn = true,
                    });

                    // Show rebuilding animation.
                    player.Animate("paradox-teleport-in");
                });

                // Progress mission 1047
                var missionInventory = player.GetComponent<MissionInventoryComponent>();
                missionInventory.ScriptAsync(1491, gameObject.Lot);
            });
        }
    }
}
