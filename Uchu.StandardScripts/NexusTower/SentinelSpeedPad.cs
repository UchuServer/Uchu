using System.Numerics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("L_NT_SENTINELWALKWAY_SERVER.lua")]
    public class SentinelSpeedPad : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public SentinelSpeedPad(GameObject gameObject) : base(gameObject)
        {
            // https://lu.lcdruniverse.org/explorer/objects/12041
            // They all have a Flag and a Name on the LDF
            // Flag between 1917 and 1945, no repeats
            // Name between Walk1 and Walk9, with repeats
            // These values don't seem to be relevant for the server

            if (!gameObject.TryGetComponent<PhantomPhysicsComponent>(out var phantomPhysicsComponent))
                return;

            // Create physics effect to accelerate player
            phantomPhysicsComponent.IsEffectActive = true;
            phantomPhysicsComponent.EffectType = PhantomPhysicsEffectType.Push;
            phantomPhysicsComponent.EffectAmount = 115f; // This is accurate, found in captures

            // Pointing in the direction of the arrows
            var direction = Vector3.Transform(new Vector3(-1, 0, 0),
                GameObject.Transform.Rotation);
            phantomPhysicsComponent.EffectDirection = direction;

            var physics = gameObject.GetComponent<PhysicsComponent>();
            Listen(physics.OnEnter, other =>
            {
                if (other.GameObject is not Player player)
                    return;

                // Progress mission 1047 and 1331
                var missionInventory = player.GetComponent<MissionInventoryComponent>();
                missionInventory.ScriptAsync(1492, gameObject.Lot);
                missionInventory.ScriptAsync(1861, gameObject.Lot);
            });
        }
    }
}
