using Uchu.Core.Resources;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/ai/np/l_npc_np_spaceman_bob.lua
    /// </summary>
    [ScriptName("l_npc_np_spaceman_bob.lua")]
    public class BobMission : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public BobMission(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<MissionGiverComponent>(out var missionGiverComponent)) return;
            
            // Listen to the imagination mission completing.
            Listen(missionGiverComponent.OnMissionOk, async message =>
            {
                var (missionId, isComplete, _, responder) = message;
                if (missionId == (int) MissionId.YourCreativeSpark 
                    && isComplete && responder.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                {
                    responder.GetComponent<DestroyableComponent>().Imagination = 6;
                    await missionInventory.ScriptAsync(980, 4009);
                }
            });
        }
    }
}