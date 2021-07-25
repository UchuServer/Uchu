using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Luz;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NexusTower
{
    [ScriptName("L_NT_ASSEMBLYTUBE_SERVER.lua")]
    public class AssemblyTube : ObjectScript
    {
        public AssemblyTube(GameObject gameObject) : base(gameObject)
        {
            // Get info from object settings
            var group = (string) gameObject.Settings["teleGroup"];
            var cinematic = (string) gameObject.Settings["Cinematic"];
            var animation = gameObject.Settings.ContainsKey("OverrideAnim")
                ? (string) gameObject.Settings["OverrideAnim"]
                : "tube-sucker";
            // nexus-tube-up also exists, don't know where it is used

            if (!gameObject.TryGetComponent<PhysicsComponent>(out var phys))
                return;

            Listen(phys.OnEnter, other =>
            {
                // Should only teleport players
                if (!(other.GameObject is Player player))
                    return;

                // Find teleporter target location
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
                    PathName = cinematic,
                    LeadIn = 0.5f,
                });

                player.Animate(animation);

                // Default time in case camera path can't be found in luz file
                var time = 5f;
                // Find total camera path (cinematic) duration
                if (Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(p =>
                    p is LuzCameraPath && p.PathName == cinematic) is LuzCameraPath cameraPath)
                    time = cameraPath.Waypoints.Sum(point => ((LuzCameraWaypoint) point).Time);

                // At the end of the cinematic, rebuild the player at the target location
                Task.Run(async () =>
                {
                    await Task.Delay((int) (time * 1000) + 0);
                    player.Teleport(target.Transform.Position);
                    player.Animate("tube-resurrect");
                });
            });
        }
    }
}
