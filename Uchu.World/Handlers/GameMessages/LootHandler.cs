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
    }
}