using System;
using Uchu.StandardScripts.Base;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.General
{
    /// <summary>
    /// Wishing well script, for both the one in Nexus Tower
    /// and the ones in other worlds.
    /// </summary>
    [ScriptName("ScriptComponent_1536_script_name__removed")]
    [ScriptName("ScriptComponent_1195_script_name__removed")]
    public class WishingWell : GenericActivityManager
    {
        private const int CooldownTime = 10000;

        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public WishingWell(GameObject gameObject) : base(gameObject)
        {
            var random = new Random();
            Listen(gameObject.OnInteract, player =>
            {
                // Register player as activity user
                this.AddActivityUser(player);
                // Charge the cost (1 red Imaginite)
                this.ChargeActivityCost(player);
                // Random value to determine loot matrix
                this.SetActivityUserData(player, 1, random.Next(1, 999));
                // Drop rewards
                this.DistributeActivityRewards(player);

                // Start the cooldown
                player.Message(new NotifyClientObjectMessage
                {
                    Associate = gameObject,
                    Name = "StartCooldown",
                });

                // Stop the cooldown when the time is up
                this.Zone.Schedule(() =>
                {
                    player.Message(new NotifyClientObjectMessage
                    {
                        Associate = gameObject,
                        Name = "StopCooldown",
                    });
                }, CooldownTime);

                // Terminate interaction with player
                player.Message(new TerminateInteractionMessage
                {
                    Associate = player,
                    Terminator = gameObject,
                    Type = TerminateType.FromInteraction,
                });
            });
        }
    }
}
