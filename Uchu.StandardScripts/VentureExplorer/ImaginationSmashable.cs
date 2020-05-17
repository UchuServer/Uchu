using System;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.World;
using Uchu.World.Scripting.Native;

namespace Uchu.StandardScripts.VentureExplorer
{
    /// <summary>
    ///     LUA Reference: l_ag_imag_smashable.lua
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
                if (!(obj is GameObject gameObject)) return;

                if (HasLuaScript(gameObject, ScriptName))
                {
                    MountDrops(gameObject);
                }
            });
            
            return Task.CompletedTask;
        }

        public void MountDrops(GameObject gameObject)
        {
            if (!gameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent))
            {
                Logger.Error("Failed to find component!");
                
                return;
            }

            if (gameObject.TryGetComponent<LootContainerComponent>(out var container))
            {
                container.Restrict(async (owner, lot) =>
                {
                    var missions = owner.GetComponent<MissionInventoryComponent>();

                    if (!await missions.HasMissionAsync(308)) return false;
                    
                    var inventory = owner.GetComponent<InventoryManagerComponent>();

                    return inventory.CountItems(lot) > 0;
                });
            }
            
            Listen(destructibleComponent.OnSmashed, (killer, owner) =>
            {
                if (owner.GetComponent<Stats>().MaxImagination == default) return;

                //
                // Spawn imagination drops
                //
                
                for (var i = 0; i < _random.Next(1, 3); i++)
                {
                    var loot = InstancingUtilities.InstantiateLoot(Lot.Imagination, owner, gameObject, gameObject.Transform.Position);

                    Start(loot);
                }

                var random = _random.Next(0, 26);
                
                if (random != 1) return;
                    
                //
                // Spawn crate chicken
                //
                    
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