using System.Linq;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Client;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.ForbiddenValley
{
    /// <summary>
    /// Native implementation of scripts/ai/fv/l_act_bounce_over_wall.lua
    /// </summary>
    [ScriptName("l_act_bounce_over_wall.lua")]
    public class GateCrasher : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public GateCrasher(GameObject gameObject) : base(gameObject)
        {
            // Load physics object information from CDClient.
            var phantomPhysicsComponentId = gameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
            var cdcComponent = ClientCache.Find<Core.Client.PhysicsComponent>(phantomPhysicsComponentId);
            var assetPath = cdcComponent?.Physicsasset;

            // Configure physics object.
            var physics = gameObject.AddComponent<PhysicsComponent>();
            physics.SetPhysicsByPath(assetPath);

            // Listen for players entering.
            Listen(physics.OnEnter, async other =>
            {
                if (!(other.GameObject is Player player)) return;
                if (!player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent)) return;
                await missionInventoryComponent.ScriptAsync(1241, gameObject.Lot);
            });
        }
    }
}