using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class LootHandler : HandlerGroup
    {
        [PacketHandler(RunTask = true)]
        public void PickupCurrencyHandle(PickupCurrencyMessage message, Player player)
        {
            if (message.Currency > player.EntitledCurrency)
            {
                Logger.Error($"{player} is trying to pick up more currency than they are entitled to.");
                return;
            }

            player.EntitledCurrency -= message.Currency;
            player.Currency += message.Currency;
        }

        [PacketHandler]
        public async Task PickupItemHandle(PickupItemMessage message, Player player)
        {
            if (message.Loot == default)
            {
                Logger.Error($"{player} is trying to pick up invalid item.");
                return;
            }

            Object.Destroy(message.Loot);
            
            await player.GetComponent<InventoryManager>().AddItemAsync(message.Loot.Lot, 1);
        }
    }
}