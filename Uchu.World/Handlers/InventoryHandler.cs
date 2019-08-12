using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers
{
    public class InventoryHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task HandleItemMovement(MoveItemInInventoryMessage message, Player player)
        {
            if (message.DestinationInventoryType == InventoryType.Invalid)
                message.DestinationInventoryType = message.CurrentInventoryType;

            Logger.Debug(
                $"Moving item {message.ItemId} to {message.DestinationInventoryType}:{message.NewSlot} with Code: {message.ResponseCode}"
            );
            
            await player.GetComponent<ItemInventory>().SyncItemMoveAsync(
                message.ItemId, message.NewSlot, message.DestinationInventoryType
            );
        }
    }
}