using System;
using System.Linq;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.AvantGardens
{
    [ScriptName("L_NPC_AG_COURSE_STARTER.lua")]
    public class MonumentRace : GenericActivityManager
    {
        private const int SecondsInDay = 3600 * 24;

        /// <summary>
        /// Handle the player clicking the start button.
        /// </summary>
        /// <param name="player"></param>
        private void PlayerStart(Player player)
        {
            this.MiniGameAddPlayer(player);
            this.ActivityStart();

            // Store start time (seconds of current day; unix timestamp is too big for a float)
            // Adjust for 3-second client-side countdown
            this.SetActivityValue(player, 2, Convert.ToSingle(DateTime.Now.TimeOfDay.TotalSeconds + 3));

            // Notify client that the race has started
            player.Message(new NotifyClientObjectMessage
            {
                Associate = this.GameObject,
                Name = "start_timer",
            });
        }

        /// <summary>
        /// Handle the player clicking the exit button.
        /// </summary>
        /// <param name="player"></param>
        private void PlayerExit(Player player)
        {
            // Remove player from activity
            this.UpdatePlayer(player, true);
            
            // Close scoreboard
            player.Message(new TerminateInteractionMessage
            {
                Associate = player,
                Terminator = this.GameObject,
                Type = TerminateType.FromInteraction,
            });
        }
        
        public MonumentRace(GameObject gameObject) : base(gameObject)
        {
            // Show message box on interaction
            Listen(gameObject.OnInteract, player =>
            {
                if (this.IsPlayerInActivity(player))
                    player.Message(new NotifyClientObjectMessage
                    {
                        Associate = gameObject,
                        Name = "exit",
                    });
                else
                    player.Message(new NotifyClientObjectMessage
                    {
                        Associate = gameObject,
                        Name = "start",
                    });
            });

            Listen(gameObject.OnMessageBoxRespond, (player, message) =>
            {
                // Start activity when user presses start button
                if (message.Identifier == "player_dialog_start_course" && message.Button == 1)
                    this.PlayerStart(player);
                // Stop activity when user presses exit button
                else if (message.Identifier == "player_dialog_cancel_course" && message.Button == 1)
                    this.PlayerExit(player);
            });

            var finish = Zone.GameObjects.FirstOrDefault(g => g.Lot == Lot.MonumentRaceFinishTrigger);
            if (finish == null || !finish.TryGetComponent<PhysicsComponent>(out var physicsComponent))
                return;

            // Listen for players reaching the finish line
            Listen(physicsComponent.OnEnter, other =>
            {
                if (!(other.GameObject is Player player))
                    return;

                if (!this.IsPlayerInActivity(player))
                    return;

                var start = Convert.ToInt32(this.GetActivityValue(player, 2));
                var now = Convert.ToInt32(DateTime.Now.TimeOfDay.TotalSeconds);
                // Account for the possibility of starting before midnight and ending after
                var elapsed = (now - start + SecondsInDay) % SecondsInDay;
                this.SetActivityValue(player, 1, elapsed);

                player.Message(new NotifyClientObjectMessage
                {
                    Associate = gameObject,
                    Name = "stop_timer",
                    Param2 = elapsed,
                });

                this.StopActivity(player, 0);

                // Progress missions
                if (player.TryGetComponent<MissionInventoryComponent>(out var missionComponent))
                {
                    missionComponent.MinigameAchievementAsync(this.GetActivityId(), "performact_time", -elapsed);
                    // Vector Longview mission: https://lu.lcdruniverse.org/explorer/missions/1884
                    missionComponent.ScriptAsync(2679, Lot.MonumentFinishLine);
                }
            });

            // End activity when player leaves area
            var cancelTriggers = Zone.GameObjects.Where(g => g.Lot == Lot.MonumentRaceCancelTrigger);
            foreach (var trigger in cancelTriggers)
            {
                if (!trigger.TryGetComponent<PhysicsComponent>(out var triggerPhysicsComponent))
                    return;
                Listen(triggerPhysicsComponent.OnEnter, other =>
                {
                    if (!(other.GameObject is Player player))
                        return;

                    if (!this.IsPlayerInActivity(player))
                        return;

                    this.UpdatePlayer(player, true);

                    player.Message(new NotifyClientObjectMessage
                    {
                        Associate = gameObject,
                        Name = "cancel_timer",
                    });
                });
            }
        }
    }

    [ScriptName("ScriptComponent_380_script_name__removed")]
    public class MonumentFinish : ObjectScript
    {
        public MonumentFinish(GameObject gameObject) : base(gameObject)
        {

        }
    }
}
