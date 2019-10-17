using System.Threading.Tasks;
using Uchu.Core;

namespace Uchu.World.Handlers.GameMessages
{
    public class VendorHandler : HandlerGroup
    {
        [PacketHandler]
        public async Task BuyFromVendor(BuyFromVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Buy(message.Lot, (uint) message.Count, player);
        }
        
        [PacketHandler]
        public async Task SellToVendor(SellToVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Sell(message.Item, (uint) message.Count, player);
        }
        
        [PacketHandler]
        public async Task BuybackFromVendor(BuybackFromVendorMessage message, Player player)
        {
            await message.Associate.GetComponent<VendorComponent>().Buyback(message.Item, (uint) message.Count, player);
        }
    }
}