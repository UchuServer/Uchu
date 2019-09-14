using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class InventoryHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void HandleItemMovement(MoveItemInInventoryMessage message, Player player)
        {
            if (message.DestinationInventoryType == InventoryType.Invalid)
                message.DestinationInventoryType = message.CurrentInventoryType;

            Logger.Debug(
                $"Moving item {message.ItemId} to {message.DestinationInventoryType}:{message.NewSlot} with Code: {message.ResponseCode}"
            );
            
            player.GetComponent<InventoryManager>().SyncItemMove(
                message.ItemId, message.NewSlot, message.CurrentInventoryType, message.DestinationInventoryType
            );
        }

        [PacketHandler(RunTask = true)]
        public void HandleEquipItem(EquipItemMessage message, Player player)
        {
            player.GetComponent<InventoryComponent>().EquipItem(message.Item);
        }
        
        [PacketHandler(RunTask = true)]
        public void HandleUnEquipItem(UnEquipItemMessage message, Player player)
        {
            var inventoryComponent = player.GetComponent<InventoryComponent>();
            
            Logger.Information($"UnEquip Item: {message.ItemToUnEquip} | {message.ReplacementItem}");
            
            inventoryComponent.UnEquipItem(message.ItemToUnEquip);
            
            if (message.ReplacementItem != null)
                inventoryComponent.EquipItem(message.ReplacementItem);
        }
    }
}