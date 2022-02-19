using Uchu.World;
using Uchu.World.Scripting.Native;
using System.Linq;

namespace Uchu.StandardScripts.General
{
    [ScriptName("l_touch_mission_update_server.lua")]
    public class TouchMissionUpdate : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public TouchMissionUpdate(GameObject gameObject) : base(gameObject)
        {
            if (gameObject.TryGetComponent<PhysicsComponent>(out var physicsComponent) && gameObject.Settings.TryGetValue("TouchCompleteID", out var setting) && setting is int completeID)
            {
                Listen(physicsComponent.OnEnter, (collider) => {
                    if (collider.GameObject is Player player && player.TryGetComponent<MissionInventoryComponent>(out var missionInventoryComponent))
                    {
                        foreach(var task in missionInventoryComponent.GetMission(completeID).Tasks.Where(i => i.Type == MissionTaskType.Script).ToArray())
                        {
                            missionInventoryComponent.ScriptAsync(task.TaskId, gameObject.Lot);
                        }
                    }
                });
            }
        }
    }
}