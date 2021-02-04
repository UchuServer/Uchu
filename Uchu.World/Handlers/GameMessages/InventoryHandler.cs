using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class InventoryHandler : HandlerGroup
    {
        [PacketHandler]
        public void ItemMovementHandler(MoveItemInInventoryMessage message, Player player)
        {
            if (message.DestinationInventoryType == InventoryType.Invalid)
                message.DestinationInventoryType = message.CurrentInventoryType;

            if (message.Item.Inventory.ManagerComponent.GameObject != player) return;
            
            message.Item.Slot = (uint) message.NewSlot;
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
        public async Task RemoveItemHandler(RemoveItemToInventoryMessage message, Player player)
        {
            if (!message.Confirmed || message.Item == default)
                return;
            
            await player.GetComponent<InventoryManagerComponent>()
                .RemoveItemAsync(message.Item, message.Item.Count - message.TotalItems, 
                    message.InventoryType, true);
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
            if (message.ItemToUnEquip == default)
            {
                Logger.Error($"{player} attempted to un equip invalid item.");
                
                return;
            }
            
            await message.ItemToUnEquip.UnEquipAsync();

            if (message.ReplacementItem != null)
                await message.ReplacementItem.EquipAsync();
        }
    }
}