using System.Threading;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World.Handlers.GameMessages
{
    public class LootHandler : HandlerGroup
    {
        private SemaphoreSlim Lock { get; } = new SemaphoreSlim(1, 1);

        [PacketHandler]
        public void PickupCurrencyHandler(PickupCurrencyMessage message, Player player)
        {
            if (player.TryGetComponent<CharacterComponent>(out var character))
            {
                if (message.Currency > character.EntitledCurrency)
                {
                    Logger.Error($"{player} is trying to pick up more currency than they are entitled to.");
                    return;
                }

                character.EntitledCurrency -= message.Currency;
                character.Currency += message.Currency;
            }
        }

        [PacketHandler]
        public async Task PickupItemHandler(PickupItemMessage message, Player player)
        {
            Logger.Debug($"{player} TRYING picking up {message.Loot}");
            
            await Lock.WaitAsync();

            Lot loot = default;

            try
            {
                if (message.Loot != default && message.Loot.Alive)
                {
                    loot = message.Loot.Lot;
                    
                    Object.Destroy(message.Loot);
                }
                else
                {
                    Logger.Error($"{player} is trying to pick up invalid item.");
                }
            }
            finally
            {
                Lock.Release();
            }
            
            if (loot == default)
            {
                return;
            }
            
            Logger.Debug($"{player} picking up {loot}");
            
            await player.OnLootPickup.InvokeAsync(loot);
            await player.GetComponent<InventoryManagerComponent>().AddLotAsync(loot, 1, lootType: LootType.Pickup);
        }

        [PacketHandler]
        public async Task HasBeenCollectedHandler(HasBeenCollectedMessage message, Player player)
        {
            await player.GetComponent<MissionInventoryComponent>().CollectAsync(
                message.Associate
            );
        }
    }
}