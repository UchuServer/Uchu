using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Collections;

namespace Uchu.World
{
    [Essential]
    public class InventoryManager : Component
    {
        private InventoryComponent _inventoryComponent;

        private readonly Dictionary<InventoryType, Inventory> _inventories = new Dictionary<InventoryType, Inventory>();

        public override void Instantiated()
        {
            base.Instantiated();

            _inventoryComponent = GameObject.GetComponent<InventoryComponent>();

            foreach (var value in Enum.GetValues(typeof(InventoryType)))
            {
                var id = (InventoryType) value;

                _inventories.Add(id, new Inventory(id, this));
            }
        }

        public Inventory this[InventoryType inventoryType] => _inventories[inventoryType];

        public Item GetItem(long id)
        {
            using (var ctx = new UchuContext())
            {
                var item = ctx.InventoryItems.FirstOrDefault(
                    i => i.InventoryItemId == id && i.CharacterId == Player.ObjectId
                );

                if (item == default)
                {
                    Logger.Error($"{id} is not an item on {Player}");
                    return null;
                }

                var managedItem = _inventories[(InventoryType) item.InventoryType].Items.FirstOrDefault(
                    i => i.ObjectId == id
                );

                if (managedItem == default)
                {
                    Logger.Error($"{id} is not managed on {Player}");
                }

                return managedItem;
            }
        }

        #region To Refactor

        private void MessageAddItem(InventoryItem inventoryItem, ItemComponent itemComponent, int toAdd, int source)
        {
            Player.Message(new AddItemToInventoryMessage
            {
                Associate = GameObject,
                ItemObjectId = inventoryItem.InventoryItemId,
                ItemLot = inventoryItem.LOT,
                ItemCount = (uint) toAdd,
                Slot = inventoryItem.Slot,
                Inventory = inventoryItem.InventoryType,
                ShowFlyingLoot = true,
                TotalItems = (uint) inventoryItem.Count,
                ExtraInfo = string.IsNullOrWhiteSpace(inventoryItem.ExtraInfo)
                    ? default
                    : LegoDataDictionary.FromString(inventoryItem.ExtraInfo),
                IsBound = inventoryItem.IsBound,
                IsBoundOnEquip = itemComponent.IsBOE ?? false,
                IsBoundOnPickup = itemComponent.IsBOP ?? false,
                Source = source
            });
        }

        private void MessageRemoveItem(InventoryItem inventoryItem, ItemComponent itemComponent, uint toDelete)
        {
            Player.Message(new RemoveItemToInventoryMessage
            {
                Associate = GameObject,
                Confirmed = true,
                DeleteItem = true,
                OutSuccess = false,
                ExtraInfo = string.IsNullOrWhiteSpace(inventoryItem.ExtraInfo)
                    ? default
                    : LegoDataDictionary.FromString(inventoryItem.ExtraInfo),
                ForceDeletion = true,
                InventoryType = (InventoryType) inventoryItem.InventoryType,
                ItemType = (ItemType) (itemComponent.ItemType ?? (int) ItemType.Invalid),
                ObjId = inventoryItem.InventoryItemId,
                StackCount = toDelete,
                StackRemaining = (uint) inventoryItem.Count
            });
        }

        public async Task AddItemAsync(int lot, int count = 1, InventoryType inventoryType = InventoryType.Items, int source = -1)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var componentId = await cdClient.ComponentsRegistryTable.FirstOrDefaultAsync(
                    r => r.Id == lot && r.Componenttype == (int) ReplicaComponentsId.Item
                );

                if (componentId == default)
                {
                    Logger.Error($"{lot} does not have a Item component");
                    return;
                }

                var component = await cdClient.ItemComponentTable.FirstOrDefaultAsync(
                    i => i.Id == componentId.Componentid
                );

                if (component == default)
                {
                    Logger.Error(
                        $"{lot} has a corrupted component registry. There is no Item component of Id: {componentId.Componentid}"
                    );
                    return;
                }

                var character = ctx.Characters.Include(c => c.Items)
                    .Single(c => c.CharacterId == GameObject.ObjectId);

                if (inventoryType == InventoryType.Invalid)
                {
                    inventoryType = ((ItemType) (component.ItemType ?? (int) ItemType.Consumable)).GetInventoryType();
                }
                
                var stackSize = component.StackSize ?? 1;

                var items = character.Items.Where(i =>
                    i.LOT == lot && i.InventoryType == (int) inventoryType &&
                    i.Count != stackSize
                ).ToList();
                
                items.Sort((i1, i2) => i1.Slot - i2.Slot);
                
                foreach (var item in items)
                {
                    if (item.Count == stackSize) continue;

                    var toAdd = Min(stackSize, count, (int) (stackSize - item.Count));

                    item.Count += toAdd;

                    count -= toAdd;

                    MessageAddItem(item, component, toAdd, source);

                    if (count != default) continue;
                    
                    await ctx.SaveChangesAsync();
                        
                    return;
                }

                int newSlot = default;

                if (character.Items.Count == default) newSlot = default;
                else
                {
                    var maxSlot = character.Items.Select(item => item.Slot).ToList().Max();
                    for (var i = 0; i < maxSlot; i++)
                    {
                        if (character.Items.Any(item => item.Slot == i)) continue;

                        newSlot = i;
                        break;
                    }

                    if (newSlot == default)
                    {
                        newSlot = maxSlot + 1;
                    }
                }

                var toCreate = Min(stackSize, count);
                
                var newStack = new InventoryItem
                {
                    LOT = lot,
                    Count = toCreate,
                    InventoryType = (int) inventoryType,
                    IsBound = component.IsBOP ?? false,
                    Slot = newSlot,
                    InventoryItemId = Utils.GenerateObjectId()
                };

                character.Items.Add(newStack);
                
                await ctx.SaveChangesAsync();
                
                MessageAddItem(newStack, component, count, source);

                var questInventory = Player.GetComponent<QuestInventory>();
                
                for (var i = 0; i < toCreate; i++)
                {
                    await questInventory.UpdateLotTaskAsync(lot, MissionTaskType.ObtainItem);
                }
                
                count -= toCreate;
                
                if (count == default) return;

                await AddItemAsync(lot, count, inventoryType, source);
            }
        }

        public async Task RemoveItemAsync(int lot, int count = 1, InventoryType inventoryType = InventoryType.Items)
        {
            using (var ctx = new UchuContext())
            using (var cdClient = new CdClientContext())
            {
                var componentId = await cdClient.ComponentsRegistryTable.FirstOrDefaultAsync(
                    r => r.Id == lot && r.Componenttype == (int) ReplicaComponentsId.Item
                );

                if (componentId == default)
                {
                    Logger.Error($"{lot} does not have a Item component");
                    return;
                }

                var component = await cdClient.ItemComponentTable.FirstOrDefaultAsync(
                    i => i.Id == componentId.Componentid
                );

                if (component == default)
                {
                    Logger.Error(
                        $"{lot} has a corrupted component registry. There is no Item component of Id: {componentId.Componentid}"
                    );
                    return;
                }

                var character = ctx.Characters.Include(c => c.Items)
                    .Single(c => c.CharacterId == GameObject.ObjectId);
                
                var items = character.Items.Where(i => i.LOT == lot && i.InventoryType == (int) inventoryType).ToList();
                items.Sort((i1, i2) =>  (int) i1.Count - (int) i2.Count);

                foreach (var item in items)
                {
                    var toRemove = Min(count, (int) item.Count);

                    item.Count -= toRemove;
                    
                    count -= toRemove;

                    MessageRemoveItem(item, component, (uint) toRemove);

                    if (item.Count == default)
                    {
                        await DisassembleItemAsync(item);
                    }

                    if (count != default) continue;
                    
                    await ctx.SaveChangesAsync();
                        
                    return;
                }

                Logger.Error(
                    $"Trying to remove {lot} x {count} when {GameObject} does not have that many of {lot} in their {inventoryType} inventory"
                );
            }
        }

        public async Task SyncItemMoveAsync(long itemId, int newSlot, InventoryType newInventoryType)
        {
            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.Include(c => c.Items)
                    .Single(c => c.CharacterId == GameObject.ObjectId);

                var item = character.Items.FirstOrDefault(i => i.InventoryItemId == itemId);

                if (item == default)
                {
                    Logger.Error(
                        $"Trying to sync an item movement for Item: {itemId} to Slot: {newSlot}, with an item that does not exist"
                    );
                    
                    return;
                }
                
                item.Slot = newSlot;
                item.InventoryType = (int) newInventoryType;

                await ctx.SaveChangesAsync();
            }
        }
 
        private async Task DisassembleItemAsync(InventoryItem inventoryItem)
        {
            if (string.IsNullOrWhiteSpace(inventoryItem.ExtraInfo)) return;
            
            var data = LegoDataDictionary.FromString(inventoryItem.ExtraInfo);
            if (data.TryGetValue("assemblyPartLOTs", out var list))
            {
                using (var cdClient = new CdClientContext())
                {
                    foreach (int lot in (LegoDataList) list)
                    {
                        var componentId = await cdClient.ComponentsRegistryTable.FirstOrDefaultAsync(
                            r => r.Id == lot && r.Componenttype == (int) ReplicaComponentsId.Item
                        );

                        if (componentId == default)
                        {
                            Logger.Error($"{lot} does not have a Item component");
                            return;
                        }

                        var component = await cdClient.ItemComponentTable.FirstOrDefaultAsync(
                            i => i.Id == componentId.Componentid
                        );

                        if (component == default)
                        {
                            Logger.Error(
                                $"{lot} has a corrupted component registry. There is no Item component of Id: {componentId.Componentid}"
                            );
                            return;
                        }

                        await AddItemAsync(
                            lot, 1,
                            ((ItemType) (component.ItemType ?? (int) ItemType.Brick)).GetInventoryType(),
                            inventoryItem.LOT
                        );
                    }
                }
            }
        }
        
        #endregion

        private static int Min(params int[] nums)
        {
            return nums.Min();
        }
    }
}