using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class VendorHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task BuyFromVendorHandler(BuyFromVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Buy(message.Lot, (uint) message.Count, player);
            await player.OnVendorPurchase.InvokeAsync(message);
        }
        
        [PacketHandler]
        public async Task SellToVendorHandler(SellToVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Sell(message.Item, (uint) message.Count, player);
        }
        
        [PacketHandler]
        public async Task BuybackFromVendorHandler(BuybackFromVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Buyback(message.Item, (uint) message.Count, player);
        }
    }
}