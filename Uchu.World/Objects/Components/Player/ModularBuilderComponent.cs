using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using InfectedRose.Core;
using Uchu.Core;
using Uchu.Core.Client;
using System;
using System.Collections.Generic;

namespace Uchu.World
{
    public class ModularBuilderComponent : Component
    {
        public GameObject BasePlate { get; private set; }
        
        public bool IsBuilding { get; private set; }

        public int BuildMode { get; private set; }

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
            BuildMode = GetBuildModeForBasePlateLot(BasePlate.Lot); 

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

            // Move all models back into the players inventory
            if (inventory[InventoryType.TemporaryModels] != default)
                foreach (Item item in inventory[InventoryType.TemporaryModels].Items)
                    await inventory.MoveItemBetweenInventoriesAsync(item, item.Count, InventoryType.TemporaryModels, InventoryType.Models, showFlyingLoot: true);

            await inventory.MoveItemBetweenInventoriesAsync(model, 1, model.Inventory.InventoryType, InventoryType.TemporaryModels);
        }

        /// <summary>
        /// When the player is done with a modular build and gets a model.
        /// </summary>
        public async Task FinishBuilding(Lot[] models)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            // Disassemble all models
            foreach (Item item in inventory[InventoryType.TemporaryModels].Items)
            {
                if (item.Settings.TryGetValue("assemblyPartLOTs", out var list))
                {
                    await inventory.RemoveItemAsync(item, 1);
                    foreach (var part in (LegoDataList) list)
                        await inventory.AddLotAsync((int) part, 1, default, InventoryType.TemporaryModels);
                }
            }

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
            Lot modelLot = GetModelLotForBuildMode(BuildMode);
            await inventory.AddLotAsync(modelLot, 1, model, InventoryType.Models);

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

            // Remove all remaining items from TemporaryModels
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

        private static int GetBuildModeForBasePlateLot(Lot lot)
        {
            // Modes are stored as bit flags in an integer
            // 1: Not Customized, 2: Model, 4: Unused
            // 8: Rocket, 16: Car (old), 32: Car (mod)
            // 64: Car module, 128: Unused, 256: Pet

            var dict = new Dictionary<Lot, int>()
            {
                { 4, 8 },     // "UGG - New Rocket Bay" (Rockets - Probably unused)
                { 8044, 32 }, // "UGG - Modular Car Garage 6 (current newest)" (Cars)
                { 9861, 32 }, // "Build Border Gnarled Forest Car" (Cars - Probably unused) 
                { 9980, 8 },  // "Build Border for Nimbus Station" (Rockets)
                { 10047, 8 }, // "Build border for LUPs station Rocket" (Rockets - Probably unused)
            };

            return dict[lot];
        }

        private static Lot GetModelLotForBuildMode(int mode)
        {
            var dict = new Dictionary<int, Lot>()
            {
                { 8, 6416 },  // Custom Modular Rocket Ship
                { 32, 8092 }, // Custom Racing Car
            };

            return dict[mode];
        }
    }
}
