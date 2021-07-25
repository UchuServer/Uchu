using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Luz;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("L_NT_PARADOXTELE_SERVER.lua")]
    public class ParadoxTeleporter : ObjectScript
    {
        public ParadoxTeleporter(GameObject gameObject) : base(gameObject)
        {
            // Get info from object settings
            var group = (string) gameObject.Settings["teleGroup"];
            var cinematic = (string) gameObject.Settings["Cinematic"];

            // All these must have a physics component to detect the player entering
            if (!gameObject.TryGetComponent<PhysicsComponent>(out var phys))
                return;

            Listen(phys.OnEnter, other =>
            {
                // Should only teleport players
                if (!(other.GameObject is Player player))
                    return;

                // Find teleporter target location
                var target = Zone.GameObjects.FirstOrDefault(obj => obj.Settings.TryGetValue("groupID", out var group2)
                                                       && ((string) group2).Split(";").Contains(group));
                if (target == null)
                    return;

                // Show camera path. Lock player so animation can't be canceled.
                player.Message(new PlayCinematicMessage
                {
                    Associate = player,
                    LeadIn = 1f,
                    LockPlayer = true,
                    PathName = cinematic,
                    StartTimeAdvance = -1f,
                });

                player.Animate("paradoxdeath");
                // paradox-teleport-in and nexusteleport also exist, don't know if they're used somewhere else

                // Default time in case camera path can't be found in luz file
                var time = 1f;
                // Find total camera path (cinematic) duration
                if (Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(p =>
                    p is LuzCameraPath && p.PathName == cinematic) is LuzCameraPath cameraPath)
                    time = cameraPath.Waypoints.Sum(point => ((LuzCameraWaypoint) point).Time);

                // At the end of the cinematic, rebuild the player at the target location
                Task.Run(async () =>
                {
                    await Task.Delay((int) (time * 1000) + 500);
                    player.Teleport(target.Transform.Position);
                    player.Animate("paradox-teleport-in");
                });
            });
        }
    }
}
