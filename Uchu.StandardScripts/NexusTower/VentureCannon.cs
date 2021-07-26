using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1519_script_name__removed")]
    public class VentureCannon : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public VentureCannon(GameObject gameObject) : base(gameObject)
        {
            // Get info from object settings
            var group = (string) gameObject.Settings["teleGroup"];
            var enterCinematic = (string) gameObject.Settings["EnterCinematic"];

            // Values from scripts/02_client/map/nt/l_nt_venture_cannon_client.lua
            const string enterAnimation = "scale-down";
            const string exitAnimation = "venture-cannon-out";

            // Listen for interactions
            Listen(gameObject.OnInteract, player =>
            {
                // Find target location
                var target = Zone.GameObjects.FirstOrDefault(obj =>
                    obj.Settings.TryGetValue("groupID", out var group2)
                    && ((string) group2).Split(";").Contains(group));

                if (target == null)
                    return;

                // Show camera path. Lock player so animation can't be canceled.
                player.Message(new PlayCinematicMessage
                {
                    Associate = player,
                    LockPlayer = true,
                    PathName = enterCinematic,
                });

                player.Animate(enterAnimation);

                // Teleport player and show animation after 1 second
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    player.Teleport(target.Transform.Position, target.Transform.Rotation);
                    player.Animate(exitAnimation);
                });
            });
        }
    }
}
