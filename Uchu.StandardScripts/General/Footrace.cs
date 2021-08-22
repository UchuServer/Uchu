using System.Linq;
using Uchu.Core.Resources;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    [ScriptName("L_ACT_BASE_FOOT_RACE.lua")]
    public class Footrace : GenericActivityManager
    {
        public Footrace(GameObject gameObject) : base(gameObject)
        {
            // Quit footrace when player interacts with launchpad
            foreach (var launchpad in Zone.GameObjects.Where(
                g => g.TryGetComponent<RocketLaunchpadComponent>(out _)))
            {
                Listen(launchpad.OnInteract, player =>
                {
                    this.EndActivity(player);
                    player.Message(new NotifyClientObjectMessage
                    {
                        Associate = gameObject,
                        Name = "stop_timer",
                    });
                });
            }

            // Quit footrace when player leaves zone (can be logging out)
            Listen(Zone.OnPlayerLeave, this.EndActivity);

            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, (arguments, message) =>
                {
                    var args = arguments.Split('_');
                    if (args.Length < 2)
                        return;
                    if (args[0] == "updatePlayer")
                    {
                        // Player is not in activity yet
                        this.UpdatePlayer(player);
                    }
                    else if (this.IsPlayerInActivity(player))
                    {
                        // Player is in activity
                        switch (args[0])
                        {
                            case "initialActivityScore":
                                // Set "in footrace" flag
                                player.GetComponent<CharacterComponent>().SetFlagAsync(Flag.InFootrace, true);

                                // Set initial score (isn't actually used later on, as the final score is sent by
                                // the client when the player completes the footrace)
                                this.InitialActivityScore(player, message.Param1, 1);
                                break;
                            case "updatePlayerTrue":
                                // End activity
                                this.EndActivity(player);
                                break;
                            case "PlayerWon":
                                // Player successfully completed footrace
                                player.GetComponent<CharacterComponent>().SetFlagAsync(Flag.InFootrace, false);

                                this.StopActivity(player, 0, message.Param1);
                                break;
                        }
                    }
                });
            });
        }

        /// <summary>
        /// End the footrace, removing the player from the activity.
        /// </summary>
        /// <param name="player">Player to end the activity for.</param>
        private void EndActivity(Player player)
        {
            player.GetComponent<CharacterComponent>().SetFlagAsync(Flag.InFootrace, false);
            Zone.Schedule(() =>
            {
                // funny race condition (foot-race condition, if you will)
                // client sends updatePlayerTrue before PlayerWon (see line 58 and 62)
                // because the player is removed from the activity, getting their score later on returns 0
                // another fix would be to remember the score after the player has left the activity
                this.UpdatePlayer(player, true);
            }, 2000);
        }
    }
}
