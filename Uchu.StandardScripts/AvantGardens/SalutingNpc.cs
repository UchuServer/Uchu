using System.Threading.Tasks;
using Uchu.World.Scripting;

namespace StandardScripts.AvantGardens
{
    /// <summary>
    ///     LUA Reference: l_ag_saluting_npcs.lua
    /// </summary>
    public class SalutingNpc : Script
    {
        private const string ScriptName = "l_ag_saluting_npcs.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName))
            {
                gameObject.OnEmoteReceived.AddListener((emoteId, player) =>
                {
                    if (emoteId == 356)
                    {
                        gameObject.Animate("salutePlayer");
                    }
                    
                    return Task.CompletedTask;
                });
            }
            
            return Task.CompletedTask;
        }
    }
}