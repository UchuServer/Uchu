using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using InfectedRose.Core;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    public class ModularBuilderComponent : Component
    {
        public GameObject BasePlate { get; private set; }
        
        public bool IsBuilding { get; private set; }

        /// <summary>
        /// Called when a modular build is completed.
        /// </summary>
        public Event<Lot[]> OnBuildFinished { get; }
        
        protected ModularBuilderComponent()
        {
            this.OnBuildFinished = new Event<Lot[]>();
        }

        public async Task StartBuildingAsync(StartBuildingWithItemMessage message)
        {
            if (!(GameObject is Player player))
                return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            var thinkingHat = inventory[InventoryType.Items].Items.First(i => i.Lot == 6086);

            await thinkingHat.EquipAsync(true);
            
            IsBuilding = true;
            BasePlate = message.Associate;
            
            player.Message(new StartArrangingWithItemMessage
            {
                Associate = GameObject,
                FirstTime = message.FirstTime,
                BuildArea = message.Associate,
                StartPosition = Transform.Position,
                
                SourceBag = message.SourceBag,
                Source = message.Source,
                SourceLot = message.SourceLot,
                SourceType = 8,
                
                Target = message.Target,
                TargetLot = message.TargetLot,
                TargetPosition = message.TargetPosition,
                TargetType = message.TargetType
            });
        }
        
        /// <summary>
        /// When the player puts a model (not a part) into the builder while already being in build mode.
        /// </summary>
        public async Task StartBuildingWithModel(Item model) {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            if (model.Settings.TryGetValue("assemblyPartLOTs", out var list))
            {
                await inventory.RemoveItemAsync(model, 1);

                foreach (var part in (LegoDataList) list)
                    await inventory.AddLotAsync((int) part, 1, default, InventoryType.TemporaryModels);
            }
        }

        /// <summary>
        /// When the player is done with a modular build and gets a model.
        /// </summary>
        public async Task FinishBuilding(Lot[] models)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            // Remove all the items that were used for building this module
            foreach (var lot in models)
            {
                await inventory.RemoveLotAsync(lot, 1, InventoryType.TemporaryModels);
            }

            // Create the rocket.
            var model = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(models.Select(s => s.Id))
            };
            await inventory.AddLotAsync(6416, 1, model, InventoryType.Models);

            // Finish the build.
            await ConfirmFinish();
            await this.OnBuildFinished.InvokeAsync(models);
        }
        
        /// <summary>
        /// When the player leaves the build mode either through exiting or being done.
        /// </summary>
        public void DoneArranging(DoneArrangingWithItemMessage message)
        {
        }

        /// <summary>
        /// When the player picks up a part from the ground.
        /// </summary>
        public async Task Pickup(Lot lot)
        {
            if (!(GameObject is Player player)) return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            var item = inventory[InventoryType.TemporaryModels].Items.First(i => i.Lot == lot);

            await item.EquipAsync(true);
        }

        public async Task ConfirmFinish()
        {
            if (!(GameObject is Player player))
                return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            if (inventory[InventoryType.TemporaryModels] != null)
            {
                foreach (var temp in inventory[InventoryType.TemporaryModels].Items)
                {
                    await inventory.MoveItemBetweenInventoriesAsync(
                        temp,
                        temp.Count,
                        InventoryType.TemporaryModels,
                        InventoryType.Models,
                        showFlyingLoot: true
                    );
                }
            }
            
            var thinkingHat = inventory[InventoryType.Items].Items.First(i => i.Lot == 6086);
            await thinkingHat.UnEquipAsync();
            
            player.Message(new FinishArrangingWithItemMessage
            {
                Associate = GameObject,
                BuildArea = BasePlate
            });
            
            IsBuilding = false;
        }
    }
}
