using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General.DeathPlane
{
    /// <summary>
    /// Native implementation of scripts/ai/act/l_act_shark_player_death_trigger.lua
    /// </summary>
    [ScriptName("l_act_shark_player_death_trigger.lua")]
    public class SharkDeathPlane : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SharkDeathPlane(GameObject gameObject) : base(gameObject)
        {
            var physics = gameObject.GetComponent<PhysicsComponent>();
            if (physics == default) return;
            Listen(physics.OnEnter, other =>
            {
                if (!(other.GameObject is Player player)) return;
                var character = player.GetComponent<CharacterComponent>();
                var missionInventoryComponent = player.GetComponent<MissionInventoryComponent>();
                Task.Run(async () => 
                {
                    //achievements require this order
                    await missionInventoryComponent.ScriptAsync(665, gameObject.Lot);
                    await missionInventoryComponent.ScriptAsync(664, gameObject.Lot);
                    await missionInventoryComponent.ScriptAsync(663, gameObject.Lot);
                });
                Task.Run(async () =>
                {
                    await player.GetComponent<DestructibleComponent>().SmashAsync(gameObject, animation: "big-shark-death");
                });
            });
        }
    }
}