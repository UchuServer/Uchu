using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting;

namespace StandardScripts.VentureExplorer
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
                gameObject.GetComponent<MissionGiverComponent>().OnMissionOk.AddListener(message =>
                {
                    var (missionId, isComplete, _, responder) = message;
                    
                    if (missionId != 173 || !isComplete) return Task.CompletedTask;

                    responder.GetComponent<Stats>().Imagination = 6;

                    responder.GetComponent<MissionInventoryComponent>().CompleteMission(664);
                    
                    return Task.CompletedTask;
                });
            }

            return Task.CompletedTask;
        }
    }
}