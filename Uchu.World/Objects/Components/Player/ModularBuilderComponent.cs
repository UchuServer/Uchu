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

        public BuildMode Mode { get; private set; }

        private Lot[] CurrentModel { get; set; }

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
            Mode = GetBuildModeForBasePlateLot(BasePlate.Lot); 

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
                var legoDataList = (LegoDataList) list;

                await GetBackOldModel();
                await inventory.RemoveItemAsync(model);

                // Remember the model that the player put into the builder
                CurrentModel = new Lot[legoDataList.Count];
                for(var i = 0; i < legoDataList.Count; i++)
                {
                    Lot part = (int)legoDataList[i];
                    CurrentModel[i] = part;
                    await inventory.AddLotAsync((int) part, 1, default, InventoryType.TemporaryModels);
                }
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
                await inventory.RemoveLotAsync(lot, 1, InventoryType.TemporaryModels);

            await CreateNewModel(models);

            // Don't give back the original model if the user made a new one.
            CurrentModel = null;

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
            var item = inventory.FindItem(lot, InventoryType.TemporaryModels);

            await item.EquipAsync(true);
        }

        public async Task ConfirmFinish()
        {
            if (!(GameObject is Player player))
                return;
            
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            await GetBackOldModel();

            // Remove all remaining items from TemporaryModels
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

        private async Task CreateNewModel(Lot[] parts)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            var model = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(parts.Select(s => s.Id))
            };

            Lot modelLot = GetModelLotForBuildMode(Mode);
            await inventory.AddLotAsync(modelLot, 1, model, InventoryType.Models);
        }

        private async Task GetBackOldModel()
        {
            if (CurrentModel == null)
                return;

            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            var parts = new Item[CurrentModel.Length];
            var model = CurrentModel;

            // Remove model so it doesn't get duplicated
            CurrentModel = null;

            // Check if all parts are still available in the players inventory
            for (var i = 0; i < parts.Length; i++)
            {
                Lot lot = model[i];

                var item = inventory.FindItem(lot, InventoryType.TemporaryModels);
                if (item == null)
                    item = inventory.FindItem(lot, InventoryType.Models);

                if (item == null)
                    return;

                parts[i] = item;
            }

            foreach (var part in parts)
                await inventory.RemoveItemAsync(part, 1);

            await CreateNewModel(model);
        }

        private static BuildMode GetBuildModeForBasePlateLot(Lot lot)
        {
            switch(lot) {
                case Lot.NewRocketBay:
                case Lot.NimbusRocketBuildBorder:
                case Lot.LupRocketBuildBorder:
                    return BuildMode.Rocket;
                case Lot.CarBuildBorder:
                case Lot.GnarledForestCarBuildBorder:
                    return BuildMode.Car;
                default:
                    return BuildMode.NotCustomized;
            }
        }

        private static Lot GetModelLotForBuildMode(BuildMode mode)
        {
            switch(mode) {
                case BuildMode.Rocket:
                    return Lot.ModularRocket;
                case BuildMode.Car:
                    return Lot.ModularCar;
                default:
                    return default;
            }
        }
    }
}
