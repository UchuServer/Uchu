using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("ScriptComponent_1541_script_name__removed")]
    public class VentureSpeedPad : ObjectScript
    {
        public VentureSpeedPad(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent))
                return;

            Listen(physicsComponent.OnEnter, async other =>
            {
                if (!(other.GameObject is Player player))
                    return;

                var skill = player.GetComponent<SkillComponent>();

                // This skill gives a speed boost for 4 seconds
                await skill.CalculateSkillAsync(927, player);
            });
        }
    }
}
