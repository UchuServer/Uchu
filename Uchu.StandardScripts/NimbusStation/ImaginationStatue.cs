using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.NimbusStation
{
    [ScriptName("l_ns_qb_imagination_statue.lua")]
    public class ImaginationStatue : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        private bool Active = false;
        public ImaginationStatue(GameObject gameObject) : base(gameObject)
        {
            
            if (gameObject.TryGetComponent<QuickBuildComponent>(out var quickBuildComponent))
            {
                Listen(quickBuildComponent.OnStateChange, state => 
                {
                    if (state == RebuildState.Completed && !Active && quickBuildComponent.Participants.FirstOrDefault() is Player player)
                    {
                        player.Message(new TerminateInteractionMessage
                        {
                            Associate = player,
                            Terminator = gameObject,
                            Type = TerminateType.FromInteraction,
                        });
                        Active = true;
                        spawnPowerups(gameObject);
                        Task.Run(async () => 
                        {
                            await Task.Delay(10000);
                            Active = false;
                        });
                    }
                });
            }
        }
        private async void spawnPowerups(GameObject gameObject)
        {
            if (Active)
            {
                //so the lua script appears to only spawn the powerups for the activating player,
                //but i think that's a little unfair, and i made it spawn powerups for everyone
                //should i make it spawn for only the activating player?
                for (var h = 0; h < 2; h++)
                {
                    foreach (var player in gameObject.Viewers)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, player, gameObject,
                        gameObject.Transform.Position);
                        Start(loot);
                    }
                }
                await Task.Delay(1500);
                spawnPowerups(gameObject);
            }
        }
    }
}