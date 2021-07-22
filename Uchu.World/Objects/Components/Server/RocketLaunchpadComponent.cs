using System.Linq;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    [ServerComponent(Id = ComponentId.RocketLaunchComponent)]
    public class RocketLaunchpadComponent : Component
    {
        /// <summary>
        /// Target scene (spawn location) for landing.
        /// </summary>
        public string TargetScene { get; set; }
        
        /// <summary>
        /// Precondition required for landing at the
        /// alternative scene.
        /// </summary>
        public string AlternativeTargetScenePrecondition { get; set; }
        
        /// <summary>
        /// Target scene (spawn location) for landing if the
        /// alternate landing precondition is met.
        /// </summary>
        public string AlternativeTargetScene { get; set; }
        
        /// <summary>
        /// Creates the component.
        /// </summary>
        protected RocketLaunchpadComponent()
        {
            Listen(OnStart, () =>
            {
                // Load the laucnhpad information.
                var componentId = GameObject.Lot.GetComponentId(ComponentId.RocketLaunchComponent);
                var rocketLaunchpadComponent = ClientCache.Find<RocketLaunchpadControlComponent>(componentId);
                if (rocketLaunchpadComponent != default)
                {
                    this.TargetScene = rocketLaunchpadComponent.TargetScene;
                    this.AlternativeTargetScenePrecondition = rocketLaunchpadComponent.AltLandingPrecondition;
                    this.AlternativeTargetScene = rocketLaunchpadComponent.AltLandingSpawnPointName;
                }
                
                // Listen for the player requesting to launch.
                Listen(GameObject.OnInteract, OnInteract);
            });
        }

        /// <summary>
        /// Launches a player.
        /// </summary>
        /// <param name="player">Player who requested launching.</param>
        public void OnInteract(Player player)
        {
            // Get the player rocket.
            var rocket = player.GetComponent<InventoryManagerComponent>()[InventoryType.Models].Items.FirstOrDefault(
                item => item.Lot == Lot.ModularRocket
            );
            if (rocket == default)
            {
                Logger.Error($"Could not find a valid rocket for {player}");
                return;
            }

            // Equip the rocket.
            rocket.WorldState = ObjectWorldState.Attached;
            player.Message(new FireClientEventMessage
            {
                Associate = GameObject,
                Arguments = "RocketEquipped",
                Target = rocket,
                Sender = player
            });

            // Set the player as landing by rocket for the next zone.
            if (!player.TryGetComponent<CharacterComponent>(out var characterComponent))
                return;
            characterComponent.LandingByRocket = true;
            characterComponent.SpawnLocationName = this.TargetScene;
        }
    }
}
