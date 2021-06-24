using System;
using System.Threading.Tasks;
using Uchu.World;
using Uchu.World.Scripting.Native;
using Uchu.Core.Resources;
using DestructibleComponent = Uchu.World.DestructibleComponent;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// Native implementation of scripts/ai/np/l_ag_imag_smashable.lua
    /// </summary>
    [ScriptName("l_ag_imag_smashable.lua")]
    public class ImaginationSmashable : ObjectScript
    {
        /// <summary>
        /// Randomizer for spawning chickens.
        /// </summary>
        private Random _random = new Random();
        
        /// <summary>
        /// Creates the object script.
        /// </summary>
        /// <param name="gameObject">Game object to control with the script.</param>
        public ImaginationSmashable(GameObject gameObject) : base(gameObject)
        {
            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                return;
            
            // Listen to the object being smashed.
            Listen(destructibleComponent.OnSmashed, (killer, owner) =>
            {
                // Manually spawn imagination if a user has the prerequisite mission as it's not in the crate loot table.
                var missionInventory = owner.GetComponent<MissionInventoryComponent>();
                if (missionInventory != default && missionInventory.HasMission((int) MissionId.UnlockYourImagination))
                {
                    for (var i = 0; i < _random.Next(1, 3); i++)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, owner, gameObject,
                            gameObject.Transform.Position);
                        Start(loot);
                    }
                }
                
                // Spawn the crate chicken.
                var random = _random.Next(0, 26);
                if (random != 1)
                    return;
                var chicken = GameObject.Instantiate(Zone, 8114, gameObject.Transform.Position);
                Start(chicken);
                Construct(chicken);
                Task.Run(async () =>
                {
                    await Task.Delay(4000);
                    Destroy(chicken);
                });
            });
        }
    }
}