using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    ///     LUA Reference: l_npc_np_spaceman_bob.lua
    /// </summary>
    [ZoneSpecific(ZoneId.VentureExplorer)]
    public class BobMission : Script
    {
        private const string ScriptName = "l_npc_np_spaceman_bob.lua";
        
        public override Task LoadAsync()
        {
            var gameObjects = HasLuaScript(ScriptName);

            foreach (var gameObject in gameObjects)
            {
                if (!gameObject.TryGetComponent<MissionGiverComponent>(out var missionGiverComponent)) continue;
                
                Listen(missionGiverComponent.OnMissionOk, async message =>
                {
                    var (missionId, isComplete, _, responder) = message;

                    if (missionId != 173 || !isComplete) return;

                    responder.GetComponent<Stats>().Imagination = 6;

                    (responder as Player)?.SendChatMessage("COMPLETING 664");
                    
                    await responder.GetComponent<MissionInventoryComponent>().CompleteMissionAsync(664);
                });
            }

            return Task.CompletedTask;
        }
    }
}