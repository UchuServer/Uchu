using System;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Services;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.LUPLaunchpadComponent)]
    public class LUPLaunchpadComponent : Component
    {
        private int[] _zones;
        
        /// <summary>
        /// Creates the component.
        /// </summary>
        protected LUPLaunchpadComponent()
        {
            Listen(OnStart, () =>
            {
                if (!this.GameObject.Settings.TryGetValue("MultiZoneIDs", out var multiZoneIds))
                {
                    Logger.Error("No zone IDs defined on LUP launchpad");
                    return;
                }

                this._zones = ((string) multiZoneIds).Split(";").Select(int.Parse).ToArray();

                // Listen for the player requesting to open the worlds menu.
                Listen(GameObject.OnInteract, OnInteract);
            });
        }

        private Item FindRocket(Player player)
        {
            // Get the player rocket.
            var inventory = player.GetComponent<InventoryManagerComponent>();
            var rocket = inventory[InventoryType.Models].Items.FirstOrDefault(
                item => item.Lot == Lot.ModularRocket
            );
            if (rocket == default)
            {
                Logger.Error($"Could not find a valid rocket for {player}");
                return null;
            }

            return rocket;
        }

        private async void OnInteract(Player player)
        {
            var rocket = FindRocket(player);

            // Equip the rocket.
            rocket.WorldState = ObjectWorldState.Attached;

            await rocket.EquipAsync(true);

            // Show the UI with the worlds the player can go to
            player.Message(new PropertyEntranceBeginMessage
            {
                Associate = GameObject,
            });
        }

        public void ChoiceBoxResponse(Player player, int index)
        {
            var rocket = FindRocket(player);

            player.Message(new FireEventClientSideMessage
            {
                Associate = GameObject,
                Arguments = "RocketEquipped",
                Target = rocket,
                Sender = player,
            });

            // Set the player as landing by rocket for the next zone.
            if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                return;
            characterComponent.LandingByRocket = true;

            // Listen for ZonePlayer event
            Delegate listener = null;
            listener = Listen(player.OnFireServerEvent, (name, message) =>
            {
                if (name != "ZonePlayer")
                    return;

                // Player is leaving zone, so we can stop listening now
                ReleaseListener(listener);

                var _ = Task.Run(async () =>
                {
                    player.GetComponent<CharacterComponent>().LaunchedRocketFrom = Zone.ZoneId;
                    var success = await player.SendToWorldAsync((ZoneId) this._zones[index]);

                    if (!success)
                        player.SendChatMessage($"Failed to transfer to {this._zones[index]}, please try later.");
                });
            });
        }
    }
}
