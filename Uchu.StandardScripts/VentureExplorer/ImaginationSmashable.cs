using System;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World;
using Uchu.World.Scripting.Native;

using RS = Uchu.Core.Resources;
using DestructibleComponent = Uchu.World.DestructibleComponent;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    /// LUA Reference: l_ag_imag_smashable.lua
    /// </summary>
    [ZoneSpecific(1000)]
    public class ImaginationSmashable : NativeScript
    {
        private const string ScriptName = "l_ag_imag_smashable.lua";
        
        private Random _random;
        
        public override Task LoadAsync()
        {
            _random = new Random();
            
            foreach (var gameObject in HasLuaScript(ScriptName))
            {
                MountDrops(gameObject);
            }

            Listen(Zone.OnObject, obj =>
            {
                if (!(obj is GameObject gameObject))
                    return;

                if (HasLuaScript(gameObject, ScriptName))
                {
                    MountDrops(gameObject);
                }
            });
            
            return Task.CompletedTask;
        }

        /// <summary>
        /// Handles loot drops by ensuring the player has accepted the bob mission before allowing imagination to spawn.
        /// Also spawns chickens with a 1/26 chance.
        /// </summary>
        /// <param name="gameObject">The game object to set the restrictions for</param>
        private void MountDrops(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                return;

            Listen(destructibleComponent.OnSmashed, (killer, owner) =>
            {
                // Manually spawn imagination if a user has the prerequisite mission as it's not in the crate LT
                var missionInventory = owner.GetComponent<MissionInventoryComponent>();
                if (missionInventory != default && missionInventory.HasMission((int) RS.Mission.UnlockYourImagination))
                {
                    for (var i = 0; i < _random.Next(1, 3); i++)
                    {
                        var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, owner, gameObject,
                            gameObject.Transform.Position);
                        Start(loot);
                    }
                }
                
                // Spawn crate chicken
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