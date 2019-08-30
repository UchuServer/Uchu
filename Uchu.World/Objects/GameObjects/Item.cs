using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;

namespace Uchu.World
{
    [Unconstructed]
    public class Item : GameObject
    {
        private uint _count;

        private bool _equipped;

        private bool _bound;

        private uint _slot;
        
        public ItemComponent ItemComponent { get; private set; }
        
        public Inventory Inventory { get; private set; }
        
        public Player Player { get; private set; }
        
        public uint Count
        {
            get => _count;
            set
            {
                _count = value;

                Task.Run(UpdateCount);
            }
        }

        /// <summary>
        /// Is this item equipped? Warning: Should only be set as a response to a client request!
        /// </summary>
        public bool Equipped
        {
            get => _equipped;
            set
            {
                _equipped = value;

                Task.Run(UpdateEquippedStatus);
            }
        }

        public bool Bound
        {
            get => _bound;
            set
            {
                _bound = value;

                Task.Run(UpdateBoundStatus);
            }
        }

        /// <summary>
        /// The slot this item inhabits. Warning: Should only be set as a response to a client request!
        /// </summary>
        public uint Slot
        {
            get => _slot;
            set
            {
                _slot = value;

                Task.Run(UpdateSlot);
            }
        }

        public static Item Instantiate(long itemId, Inventory inventory)
        {
            using (var cdClient = new CdClientContext())
            using (var ctx = new UchuContext())
            {
                var item = ctx.InventoryItems.FirstOrDefault(
                    i => i.InventoryItemId == itemId && i.Character.CharacterId == inventory.Manager.Player.ObjectId
                );

                if (item == default)
                {
                    Logger.Error($"{itemId} is not an item on {inventory.Manager.Player}");
                    return null;
                }

                var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                    o => o.Id == item.LOT
                );

                var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                    r => r.Id == item.LOT && r.Componenttype == 11
                );

                if (cdClientObject == default || itemRegistryEntry == default)
                {
                    Logger.Error($"{itemId} [{item.LOT}] is not a valid item");
                    return null;
                }
                
                var instance = Instantiate<Item>
                (
                    inventory.Manager.Zone, cdClientObject.Name, objectId: itemId, lot: item.LOT
                );
                
                var itemComponent = cdClient.ItemComponentTable.First(
                    i => i.Id == itemRegistryEntry.Componentid
                );
                
                instance._count = (uint) item.Count;
                instance._equipped = item.IsEquipped;
                
                instance.ItemComponent = itemComponent;
                instance.Inventory = inventory;
                instance.Player = inventory.Manager.Player;

                return instance;
            }
        }

        public static Item Instantiate(int lot, Inventory inventory, uint count)
        {
            using (var cdClient = new CdClientContext())
            using (var ctx = new UchuContext())
            {
                var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                    o => o.Id == lot
                );

                var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                    r => r.Id == lot && r.Componenttype == 11
                );

                if (cdClientObject == default || itemRegistryEntry == default)
                {
                    Logger.Error($"<new item> [{lot}] is not a valid item");
                    return null;
                }
                
                var instance = Instantiate<Item>
                (
                    inventory.Manager.Zone, cdClientObject.Name, objectId: Utils.GenerateObjectId(), lot: lot
                );
                
                var itemComponent = cdClient.ItemComponentTable.First(
                    i => i.Id == itemRegistryEntry.Componentid
                );
                
                inventory.Manager.Player.Message(new AddItemToInventoryMessage
                {
                    Associate = inventory.Manager.Player,
                    Inventory = (int) inventory.InventoryType,
                    ItemCount = count,
                    TotalItems = count
                });

                return instance;
            }
        }
        
        private async Task UpdateCount()
        {
            if (_count == default)
            {
                Destroy(this);
                return;
            }
            
            using (var ctx = new UchuContext())
            {
                if (_count > ItemComponent.StackSize && ItemComponent.StackSize > 0)
                {
                    Logger.Error(
                        $"Trying to set {Lot} count to {_count}, this is beyond the item's stack-size; Setting it to stack-size"
                    );
                    
                    _count = (uint) ItemComponent.StackSize;
                }
                
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

                if (_count > item.Count) await AddCount(_count);
                else await RemoveCount(_count);
                
                item.Count = _count;
            }
        }

        private async Task AddCount(uint count)
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);
                
                Player.Message(new AddItemToInventoryMessage
                {
                    Associate = Player,
                    Inventory = item.InventoryType,
                    ItemCount = (uint) (count - item.Count),
                    TotalItems = count
                });
            }
        }

        private async Task RemoveCount(uint count)
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);
                
                Player.Message(new RemoveItemToInventoryMessage
                {
                    Associate = Player,
                    Confirmed = true,
                    DeleteItem = count == default,
                    ForceDeletion = true,
                    InventoryType = (InventoryType) item.InventoryType,
                    ItemType = (ItemType) (ItemComponent.ItemType ?? -1),
                    ObjId = ObjectId,
                    StackCount = (uint) (count - item.Count),
                    StackRemaining = count
                });
            }
        }

        private async Task RemoveFromInventory()
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

                item.Character.Items.Remove(item);

                await ctx.SaveChangesAsync();
            }
        }

        private async Task UpdateEquippedStatus()
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

                item.IsEquipped = _equipped;

                await ctx.SaveChangesAsync();
            }
        }
        
        private async Task UpdateBoundStatus()
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

                item.IsBound = _bound;

                await ctx.SaveChangesAsync();
            }
        }

        private async Task UpdateSlot()
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

                item.Slot = (int) _slot;

                await ctx.SaveChangesAsync();
            }
        }

        public override void End()
        {
            Task.Run(RemoveFromInventory);
        }
    }
}