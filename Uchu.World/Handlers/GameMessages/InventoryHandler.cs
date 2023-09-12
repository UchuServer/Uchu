using System.Threading.Tasks;
using InfectedRose.Core;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class InventoryHandler : HandlerGroup
    {
        [PacketHandler]
        public async void RequestMoveBetweenInventoriesHandler(RequestMoveItemBetweenInventoryTypesMessage message, Player player)
        {
            var inventoryManager = message.Item.Inventory.ManagerComponent;
            if (inventoryManager.GameObject != player)
                return;

            await inventoryManager.MoveItemBetweenInventoriesAsync(message.Item, message.Count, message.Source,
                message.Destination, showFlyingLoot: message.ShowFlyingLoot);

            // TODO: show animation dropping item out of the vault (https://youtu.be/hJl8WvLz6rM?t=660)
        }

        [PacketHandler]
        public void ItemMovementHandler(MoveItemInInventoryMessage message, Player player)
        {
            var inventoryManager = message.Item.Inventory.ManagerComponent;
            if (inventoryManager.GameObject != player)
                return;

            var destinationInventory = message.DestinationInventoryType == InventoryType.Invalid
                ? message.CurrentInventoryType
                : message.DestinationInventoryType;

            // If the slot is occupied, switch the items
            var itemToSwap = inventoryManager[destinationInventory][(uint)message.NewSlot];
            if (itemToSwap != null)
            {
                itemToSwap.Slot = message.Item.Slot;
            }

            message.Item.Slot = (uint)message.NewSlot;
        }

        [PacketHandler]
        public async Task ItemMoveBetweenInventoriesHandler(MoveItemBetweenInventoryTypesMessage message, Player player)
        {
            var inventory = player.GetComponent<InventoryManagerComponent>();

            // Sometimes an explicit item is provided, if it is available we prefer to use that to mimic the move of
            // the supplied slot, otherwise we find the first item that matches the criteria and move that
            if (message.Item == null)
            {
                await inventory.MoveLotBetweenInventoriesAsync(message.Lot, message.StackCount, message.SourceInventory,
                    message.DestinationInventory);
            }
            else
            {
                await inventory.MoveItemBetweenInventoriesAsync(message.Item, message.StackCount, message.SourceInventory,
                    message.DestinationInventory);
            }
        }

        [PacketHandler]
        public async Task RemoveItemHandler(RemoveItemFromInventoryMessage message, Player player)
        {
            if (!message.Confirmed || message.Item == default)
                return;

            await player.GetComponent<InventoryManagerComponent>()
                .RemoveItemAsync(message.Item, message.Item.Count - message.TotalItems,
                    message.InventoryType, true);

            // Disassemble item
            if (player.TryGetComponent<InventoryManagerComponent>(out var inventory)
                && message.Item.Settings.TryGetValue("assemblyPartLOTs", out var list))
            {
                foreach (var part in (LegoDataList)list)
                {
                    await inventory.AddLotAsync((int)part, 1);
                }
            }
        }

        [PacketHandler]
        public async Task EquipItemHandler(EquipItemMessage message, Player player)
        {
            if (message.Item == null) return;

            await message.Item.EquipAsync();
        }

        [PacketHandler]
        public async Task UnEquipItemHandler(UnEquipItemMessage message, Player player)
        {
            if (message.ItemToUnEquip == null) return;

            await message.ItemToUnEquip.UnEquipAsync();

            if (message.ReplacementItem != null)
                await message.ReplacementItem.EquipAsync();
        }

        [PacketHandler]
        public void PopEquippedItemsStateHandler(PopEquippedItemsStateMessage message, Player player)
        {
            var inventory = player.GetComponent<InventoryComponent>();
            inventory.PopEquippedItemState();

            var destroyable = player.GetComponent<DestroyableComponent>();
            destroyable.Health = destroyable.MaxHealth;
            destroyable.Armor = destroyable.MaxArmor;
            destroyable.Imagination = destroyable.MaxImagination;

            GameObject.Serialize(player);
        }
    }
}
