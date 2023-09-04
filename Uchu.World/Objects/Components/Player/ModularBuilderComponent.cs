using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Core;
using Uchu.Core;

namespace Uchu.World
{
    public class ModularBuilderComponent : Component
    {
        public GameObject BuildArea { get; private set; }

        public bool IsBuilding { get; private set; }

        private Lot[] CurrentModel = null;

        private Player Player => (Player)this.GameObject;

        /// <summary>
        /// Called when a modular build is completed.
        /// </summary>
        public Event<ModularBuildCompleteEvent> OnBuildFinished { get; }

        protected ModularBuilderComponent()
        {
            this.OnBuildFinished = new Event<ModularBuildCompleteEvent>();
        }

        /// <summary>
        /// Starts a modular builder session. Can be exited by calling <see cref="FinishBuild"/> or <see cref="ConfirmExitBuild"/>
        /// </summary>
        public void StartBuild(StartBuildingWithItemMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");
            IsBuilding = true;
            BuildArea = message.Associate;

            var inventory = Player.GetComponent<InventoryComponent>();
            inventory.PushEquippedItemState();

            Logger.Debug($"Start building with {message.Associate}");

            var sourceType = message.SourceType;
            Logger.Debug($"Source Type: {sourceType}");

            Player.Message(new StartArrangingWithItemMessage
            {
                Associate = Player,
                FirstTime = message.FirstTime,
                BuildArea = message.Associate,
                StartPosition = Player.Transform.Position,

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
        /// Sets the build mode (whatever this is).
        /// </summary>
        public void SetBuildMode(SetBuildModeMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");
            Logger.Debug($"Build mode set to {message.ModeValue}");

            Player.Message(new SetBuildModeConfirmedMessage
            {
                Associate = Player,
                Start = message.Start,
                WarnVisitors = false,
                ModePaused = message.ModePaused,
                ModeValue = message.ModeValue,
                Player = Player,
                StartPosition = message.StartPosition
            });
        }

        /// <summary>
        /// When the player puts a model (not a part) into the builder while already being in build mode.
        /// </summary>
        public async Task ModelAdded(StartArrangingWithModelMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");
            Logger.Debug($"Model added {message.Item}");

            await RetrieveCurrentModel();
            await StoreCurrentModel(message.Item);
        }

        /// <summary>
        /// Is called when a player picks up an item from the build/ground.
        /// </summary>
        public async void MoveAndEquip(ModularBuildMoveAndEquipMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");

            var inventory = Player.GetComponent<InventoryManagerComponent>();

            var item = inventory.FindItem(message.Lot, InventoryType.TemporaryModels);

            // This is just fallback and should never happen
            if (item == null)
                item = inventory.FindItem(message.Lot);

            if (item == null)
                return;

            var source = item.Inventory.InventoryType;
            var target = InventoryType.Models;
            await inventory.MoveItemBetweenInventoriesAsync(item, 1, source, target);

            // We have to find this item again because we moved it
            item = inventory.FindItem(message.Lot, target);
            if (item == null)
                return;

            await item.EquipAsync();
        }

        /// <summary>
        /// Finishes the build and assembles a model. This new model is put into the models inventory.
        /// </summary>
        public async Task FinishBuild(ModularBuildFinishMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");

            var modelLot = await CreateModel(message.Modules);

            await ExitBuild();
            await this.OnBuildFinished.InvokeAsync(new ModularBuildCompleteEvent
            {
                model = modelLot,
                parts = message.Modules
            });
        }

        /// <summary>
        /// Quits the build without assembling a models. Puts all used items back into the models inventory.
        /// </summary>
        public async void ConfirmExitBuild(BuildExitConfirmationMessage message)
        {
            Logger.Debug($"Associate: {message.Associate}");

            await RetrieveCurrentModel();
            await ExitBuild();
        }

        /// <summary>
        /// Stops the build mode and cleans up.
        /// </summary>
        private async Task ExitBuild()
        {
            // Should be removed once PopEquippedItemState is implemented
            var inventory = Player.GetComponent<InventoryManagerComponent>();
            var thinkingHat = inventory.FindItem(Lot.ThinkingHat);
            await thinkingHat.UnEquipAsync();

            await CleanupTempModels();
            ClearCurrentModel();

            Player.Message(new FinishArrangingWithItemMessage
            {
                Associate = Player,
                BuildArea = BuildArea
            });

            BuildArea = null;
            IsBuilding = false;
        }

        /// <summary>
        /// Stores a model, that the user put into the builde for later use (for example to give it back when the player exits).
        /// See <see cref="RetrieveCurrentModel"/>.
        /// </summary>
        private async Task StoreCurrentModel(Item model)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            var configParts = (LegoDataList)model.Settings["assemblyPartLOTs"];
            if (configParts == null)
                return;

            Lot[] parts = configParts.Select(part => (Lot)(int)part).ToArray();
            CurrentModel = parts;

            foreach (Lot part in parts)
            {
                await inventory.AddLotAsync(part, 1, default, InventoryType.TemporaryModels);
            }

            await inventory.RemoveItemAsync(model, 1);
        }

        /// <summary>
        /// Give back the model stored by <see cref="StoreCurrentModel"/>.
        /// </summary>
        private async Task RetrieveCurrentModel()
        {
            if (CurrentModel == null || CurrentModel.Length == 0)
                return;

            await CreateModel(CurrentModel);
            ClearCurrentModel();
        }

        /// <summary>
        /// Clears the model stored by <see cref="StoreCurrentModel"/>.
        /// </summary>
        private void ClearCurrentModel()
        {
            CurrentModel = null;
        }

        /// <summary>
        /// Moves all items from TemporaryModels to Models inventory.
        /// </summary>
        private async Task CleanupTempModels()
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();
            var tempModels = inventory[InventoryType.TemporaryModels];

            if (tempModels == null)
                return;

            // Remove all remaining items from TemporaryModels
            foreach (var temp in tempModels.Items)
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

        /// <summary>
        /// Creates a new model from the parts in the builder.
        /// </summary>
        private async Task<Lot> CreateModel(Lot[] parts)
        {
            var inventory = GameObject.GetComponent<InventoryManagerComponent>();

            var config = new LegoDataDictionary
            {
                ["assemblyPartLOTs"] = LegoDataList.FromEnumerable(parts.Select(s => s.Id))
            };

            Lot modelLot = GetModelLotForBuildAreaLot(BuildArea.Lot);
            await inventory.AddLotAsync(modelLot, 1, config, InventoryType.Models);

            foreach (Lot part in parts)
            {
                await inventory.RemoveLotAsync(part, 1);
            }

            return modelLot;
        }

        /// <summary>
        /// Gets what lot the output model should have.
        /// </summary>
        private static Lot GetModelLotForBuildAreaLot(Lot lot)
        {
            switch (lot)
            {
                case Lot.NewRocketBay:
                case Lot.NimbusRocketBuildBorder:
                case Lot.LupRocketBuildBorder:
                    return Lot.ModularRocket;
                case Lot.CarBuildBorder:
                case Lot.GnarledForestCarBuildBorder:
                    return Lot.ModularCar;
                default:
                    return default;
            }
        }

        public struct ModularBuildCompleteEvent
        {
            public Lot model { get; set; }
            public Lot[] parts { get; set; }
        }
    }
}
