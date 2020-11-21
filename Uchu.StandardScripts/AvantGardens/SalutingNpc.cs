using System.Threading.Tasks;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    /// <summary>
    ///     LUA Reference: l_ag_saluting_npcs.lua
    /// </summary>
    [ZoneSpecific(1100)]
    public class SalutingNpc : NativeScript
    {
        private const string ScriptName = "l_ag_saluting_npcs.lua";
        
        public override Task LoadAsync()
        {
            foreach (var gameObject in HasLuaScript(ScriptName))
            {
                Listen(gameObject.OnEmoteReceived, (emoteId, player) =>
                {
                    if (emoteId == 356)
                    {
                        gameObject.Animate("salutePlayer");
                    }
                    
                    return Task.CompletedTask;
                });
                
                Listen(gameObject.OnInteract, player =>
                {
                    gameObject.Animate("salutePlayer");
                });
            }
            
            return Task.CompletedTask;
        }
    }
}