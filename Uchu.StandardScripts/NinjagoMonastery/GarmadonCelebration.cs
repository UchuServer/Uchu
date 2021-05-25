using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.Core.Resources;
using Uchu.Physics;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NinjagoMonastery
{
    [ZoneSpecific(2000)]
    public class GarmadonCelebration : NativeScript
    {
        public override Task LoadAsync()
        {
            // There are two objects, one at the bridge & one at the door, only the bridge one should be used
            var gameObject = HasLuaScript("l_garmadon_celebration_server.lua").First(o =>
                ((string) o.Settings["custom_script_client"]).ToLower().EndsWith("l_garmadon_celebration_client.lua"));

            var physics = gameObject.AddComponent<PhysicsComponent>();

            var physicsObject = BoxBody.Create(
                gameObject.Zone.Simulation,
                gameObject.Transform.Position,
                gameObject.Transform.Rotation,
                new Vector3(40, 20, 5));

            physics.SetPhysics(physicsObject);

            Listen(physics.OnEnter, (other) =>
            {
                if (!(other.GameObject is Player player)) return;
                // This mission check is very likely not correct. I suspect there's a flag
                // to be set here, or maybe a hidden achievement (searched, but couldn't
                // find anything), but I don't have the original server script to check.
                // This line checks if the player has Discovered the monastery.
                var missionInventory = player.GetComponent<MissionInventoryComponent>();
                if (missionInventory.HasCompleted(2041))
                    return;
                player.TriggerCelebration(CelebrationId.LordGarmadon);
            });

            return Task.CompletedTask;
        }
    }
}