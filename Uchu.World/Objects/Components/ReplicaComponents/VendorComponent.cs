using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class VendorComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.VendorComponent;
        
        public ShopEntry[] Entries { get; set; }
        
        public Event<Lot, uint, Player> OnBuy { get; }
        
        public Event<Item, uint, Player> OnSell { get; }
        
        public Event<Item, uint, Player> OnBuyback { get; }

        protected VendorComponent()
        {
            OnBuy = new Event<Lot, uint, Player>();
            
            OnSell = new Event<Item, uint, Player>();
            
            OnBuyback = new Event<Item, uint, Player>();

            Listen(OnStart, async () =>
            {
                await SetupEntries();
                
                Listen(GameObject.OnInteract, OnInteract);
            });
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);


            // This flag is only true if construction is true
            writer.WriteBit(true); // Is Active?
            writer.WriteBit(false); // Unknown Bit
        }

        private void OnInteract(Player player)
        {
            player.Message(new OpenVendorWindowMessage
            {
                Associate = GameObject
            });
            
            player.Message(new VendorStatusUpdateMessage
            {
                Associate = GameObject,
                Entries = Entries
            });
        }

        private async Task SetupEntries()
        {
            var componentId = GameObject.Lot.GetComponentId(ComponentId.VendorComponent);

            var vendorComponent = (await ClientCache.GetTableAsync<Core.Client.VendorComponent>()).First(c => c.Id == componentId);

            var matrices = ClientCache.GetTable<Core.Client.LootMatrix>().Where(l => l.LootMatrixIndex == vendorComponent.LootMatrixIndex);

            var shopItems = new List<ShopEntry>();

            foreach (var matrix in matrices)
            {
                shopItems.AddRange(ClientCache.GetTable<LootTable>().Where(
                    l => l.LootTableIndex == matrix.LootTableIndex
                ).ToArray().Select(lootTable =>
                {
                    Debug.Assert(lootTable.Itemid != null, "lootTable.Itemid != null");
                    Debug.Assert(lootTable.SortPriority != null, "lootTable.SortPriority != null");
                    
                    return new ShopEntry
                    {
                        Lot = new Lot(lootTable.Itemid.Value),
                        SortPriority = lootTable.SortPriority.Value
                    };
                }));
            }

            Entries = shopItems.ToArray();
        }

        public async Task Buy(Lot lot, uint count, Player player)
        {
            var itemComponent = (await ClientCache.GetTableAsync<ItemComponent>()).First(
                i => i.Id == lot.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (count == default || itemComponent.BaseValue <= 0) return;

            var cost = (uint) ((itemComponent.BaseValue ?? 0) * count);
            
            if (cost > player.Currency) return;

            player.Currency -= cost;
            
            await player.GetComponent<InventoryManagerComponent>().AddItemAsync(lot, count);
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnBuy.InvokeAsync(lot, count, player);
        }

        public async Task Sell(Item item, uint count, Player player)
        {
            var itemComponent = item.ItemComponent;
            
            if (count == default || itemComponent.BaseValue <= 0) return;
            
            await player.GetComponent<InventoryManagerComponent>().MoveItemsBetweenInventoriesAsync(
                default,
                item.Lot,
                count,
                item.Inventory.InventoryType,
                InventoryType.VendorBuyback
            );

            var returnCurrency =
                Math.Floor(
                    (itemComponent.BaseValue ?? 0) *
                    (itemComponent.SellMultiplier ?? 0.1f)
                ) * count;

            player.Currency += (uint) returnCurrency;
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnSell.InvokeAsync(item, count, player);
        }

        public async Task Buyback(Item item, uint count, Player player)
        {
            var itemComponent = item.ItemComponent;
            
            if (count == default || itemComponent.BaseValue <= 0) return;

            var cost =
                (uint) Math.Floor(
                    (itemComponent.BaseValue ?? 0) *
                    (itemComponent.SellMultiplier ?? 0.1f)
                ) * count;

            if (cost > player.Currency) return;

            player.Currency -= cost;
            
            var manager = player.GetComponent<InventoryManagerComponent>();
            
            manager.RemoveItem(item.Lot, count, InventoryType.VendorBuyback);
            
            await manager.AddItemAsync(item.Lot, count);
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnBuyback.InvokeAsync(item, count, player);
        }
    }
}