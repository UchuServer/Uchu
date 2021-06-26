using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.World.Social;

namespace Uchu.StandardScripts.NexusTower
{
    /// <summary>
    /// Native implementation of scripts/02_client/map/general/l_bank_interact_client.lua
    /// </summary>
    [ScriptName("l_bank_interact_client.lua")]
    public class Vault : ObjectScript
    {
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public Vault(GameObject gameObject) : base(gameObject)
        {
            // Listen to players interacting with the vault.
            Listen(gameObject.OnInteract, player =>
            {
                UiHelper.StateAsync(player, "bank");
                player.Message(new NotifyClientObjectMessage
                {
                    Name = "OpenBank",
                    Associate = player,
                });
            });
        }

        /// <summary>
        /// Callback that is run once with the first GameObject created.
        /// </summary>
        public override void CompleteOnce()
        {
            // List to players closing the vault.
            Listen(Zone.OnPlayerLoad, player =>
            {
                Listen(player.OnFireServerEvent, (s, message) =>
                {
                    if (message.Arguments != "ToggleBank") return;
                    UiHelper.ToggleAsync(player, "ToggleBank", false);
                    player.Message(new NotifyClientObjectMessage
                    {
                        Name = "CloseBank",
                        Associate = player,
                    });
                });
            });
        }
    }
}
