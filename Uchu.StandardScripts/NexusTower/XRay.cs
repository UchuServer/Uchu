using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("L_NT_XRAY_SERVER.lua")]
    public class XRay : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public XRay(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var phys))
                return;

            // Listen for players leaving the trigger area.
            Listen(phys.OnLeave, other =>
            {
                if (!(other.GameObject is Player player))
                    return;

                // Show green glowing effect on player.
                player.GetComponent<SkillComponent>().CalculateSkillAsync(1220, player);
            });
        }
    }
}
