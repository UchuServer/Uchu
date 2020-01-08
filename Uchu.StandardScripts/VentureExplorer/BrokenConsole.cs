using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    ///     LUA Reference: l_ag_ship_player_shock_server.lua
    /// </summary>
    public class BrokenConsole : Script
    {
        private const string ScriptName = "l_ag_ship_player_shock_server.lua";
        
        public override Task LoadAsync()
        {
            var gameObjects = HasLuaScript(ScriptName);
            
            foreach (var gameObject in gameObjects)
            {
                Listen(gameObject.OnInteract, player =>
                {
                    player.Message(new ServerTerminateInteractionMessage
                    {
                        Associate = player,
                        Terminator = gameObject,
                        Type = TerminateType.FromInteraction
                    });

                    player.Animate("knockback-recovery", true);
                    
                    player.Message(new KnockbackMessage
                    {
                        Associate = player,
                        Caster = gameObject,
                        Originator = gameObject,
                        Vector = new Vector3
                        {
                            X = -20,
                            Y = 10,
                            Z = -20
                        }
                    });
                    
                    gameObject.PlayFX("console_sparks", "create", 1430);

                    Task.Run(async () =>
                    {
                        await Task.Delay(2000);
                        
                        gameObject.StopFX("console_sparks");
                    });
                });
            }
            
            return Task.CompletedTask;
        }
    }
}