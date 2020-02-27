using System;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;

namespace Uchu.World
{
    [Unconstructed]
    public class Item : GameObject
    {
        protected Item()
        {
            Listen(OnStart, () =>
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);
            });
            
            Listen(OnDestroyed, () => Task.Run(RemoveFromInventoryAsync));
            
            Listen(OnDestroyed, () => Inventory.UnManageItem(this));
        }

        public ItemComponent ItemComponent
        {
            get
            {
                using var cdClient = new CdClientContext();

                var id = Lot.GetComponentId(ComponentId.ItemComponent);

                return cdClient.ItemComponentTable.FirstOrDefault(c => c.Id == id);
            }
        }

        public Inventory Inventory { get; private set; }

        public Player Player { get; private set; }

        public InventoryItem InventoryItem
        {
            get
            {
                using var ctx = new UchuContext();
                return ctx.InventoryItems.FirstOrDefault(i => i.InventoryItemId == ObjectId);
            }
        }

        public uint Count
        {
            get
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                return (uint) info.Count;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                info.Count = value;
                
                ctx.SaveChanges();
                
                UpdateCount(value);
            }
        }

        /// <summary>
        ///     Is this item equipped?
        /// </summary>
        /// <remarks>
        ///     Should only be set as a response to a client request.
        /// </remarks>
        public bool Equipped
        {
            get
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                return info.IsEquipped;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                info.IsEquipped = value;
                
                ctx.SaveChanges();
            }
        }

        public bool Bound
        {
            get
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                return info.IsBound;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                info.IsBound = value;
                
                ctx.SaveChanges();
            }
        }

        /// <summary>
        ///     The slot this item inhabits.
        /// </summary>
        /// <remarks>
        ///     Should only be set as a response to a client request.
        /// </remarks>
        public uint Slot
        {
            get
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                return (uint) info.Slot;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

                info.Slot = (int) value;
                
                ctx.SaveChanges();
            }
        }

        public ItemType ItemType => (ItemType) (ItemComponent.ItemType ?? (int) ItemType.Invalid);

        public async Task EquipAsync(bool skipAllChecks = false)
        {
            var inventory = Player.GetComponent<InventoryComponent>();

            Equipped = true;
            
            await inventory.EquipItemAsync(this, skipAllChecks);
        }

        public async Task UnEquipAsync(bool skipAllChecks = false)
        {
            var inventory = Player.GetComponent<InventoryComponent>();

            Equipped = false;
            
            await inventory.UnEquipItemAsync(this);
        }
        
        public static Item Instantiate(long itemId, Inventory inventory)
        {
            using var cdClient = new CdClientContext();
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.FirstOrDefault(
                i => i.InventoryItemId == itemId && i.Character.CharacterId == inventory.ManagerComponent.GameObject.ObjectId
            );

            if (item == default)
            {
                Logger.Error($"{itemId} is not an item on {inventory.ManagerComponent.GameObject}");
                return null;
            }

            var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
                o => o.Id == item.LOT
            );

            var itemRegistryEntry = ((Lot) item.LOT).GetComponentId(ComponentId.ItemComponent);

            if (cdClientObject == default || itemRegistryEntry == default)
            {
                Logger.Error($"{itemId} [{item.LOT}] is not a valid item");
                return null;
            }

            var instance = Instantiate<Item>
            (
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: itemId, lot: item.LOT
            );

            if (!string.IsNullOrWhiteSpace(item.ExtraInfo)) 
                instance.Settings = LegoDataDictionary.FromString(item.ExtraInfo);

            instance.Inventory = inventory;
            instance.Player = inventory.ManagerComponent.GameObject as Player;

            return instance;
        }

        public static Item Instantiate(Lot lot, Inventory inventory, uint count, LegoDataDictionary extraInfo = default)
        {
            uint slot = default;

            for (var index = 0; index < inventory.Size; index++)
            {
                if (inventory.Items.All(i => i.Slot != index)) break;

                slot++;
            }

            return Instantiate(lot, inventory, count, slot, extraInfo);
        }

        public static Item Instantiate(Lot lot, Inventory inventory, uint count, uint slot, LegoDataDictionary extraInfo = default)
        {
            using var cdClient = new CdClientContext();
            using var ctx = new UchuContext();
            
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
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: IdUtilities.GenerateObjectId(), lot: lot
            );

            instance.Settings = extraInfo ?? new LegoDataDictionary();

            var itemComponent = cdClient.ItemComponentTable.First(
                i => i.Id == itemRegistryEntry.Componentid
            );

            instance.Inventory = inventory;
            instance.Player = inventory.ManagerComponent.GameObject as Player;

            var playerCharacter = ctx.Characters.Include(c => c.Items).First(
                c => c.CharacterId == inventory.ManagerComponent.GameObject.ObjectId
            );

            var inventoryItem = new InventoryItem
            {
                Count = count,
                InventoryType = (int) inventory.InventoryType,
                InventoryItemId = instance.ObjectId,
                IsBound = itemComponent.IsBOP ?? false,
                Slot = (int) slot,
                LOT = lot,
                ExtraInfo = extraInfo?.ToString()
            };

            playerCharacter.Items.Add(inventoryItem);

            ctx.SaveChanges();

            var message = new AddItemToInventoryMessage
            {
                Associate = inventory.ManagerComponent.GameObject,
                InventoryType = (int) inventory.InventoryType,
                Delta = count,
                TotalItems = count,
                Slot = (int) slot,
                ItemLot = lot,
                IsBoundOnEquip = itemComponent.IsBOE ?? false,
                IsBoundOnPickup = itemComponent.IsBOP ?? false,
                IsBound = inventoryItem.IsBound,
                Item = instance,
                ExtraInfo = extraInfo
            };

            (inventory.ManagerComponent.GameObject as Player)?.Message(message);

            inventory.ManageItem(instance);

            return instance;
        }

        public async Task SetCountSilentAsync(uint count)
        {
            await using var ctx = new UchuContext();
            
            if (count > ItemComponent.StackSize && ItemComponent.StackSize > 0)
            {
                Logger.Error(
                    $"Trying to set {Lot} count to {Count}, this is beyond the item's stack-size; Setting it to stack-size"
                );

                count = (uint) ItemComponent.StackSize;
            }

            var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);
            
            item.Count = count;
            
            Logger.Debug($"Setting {this}'s stack size to {item.Count}");

            if (count <= 0)
            {
                ctx.InventoryItems.Remove(item);
                
                Destroy(this);

                // Disassemble item.
                if (LegoDataDictionary.FromString(item.ExtraInfo).TryGetValue("assemblyPartLOTs", out var list))
                {
                    foreach (var part in (LegoDataList) list)
                    {
                        await Inventory.ManagerComponent.AddItemAsync((int) part, 1);
                    }
                }
            }
                
            await ctx.SaveChangesAsync();
        }
        
        private void UpdateCount(uint count)
        {
            using var ctx = new UchuContext();
            
            if (count > ItemComponent.StackSize && ItemComponent.StackSize > 0)
            {
                Logger.Error(
                    $"Trying to set {Lot} count to {count}, this is beyond the item's stack-size; setting it to stack-size"
                );

                count = (uint) ItemComponent.StackSize;
            }

            var item = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

            if (count > item.Count)
            {
                AddCount(count);
            }
            else
            {
                RemoveCount(count);
            }

            item.Count = count;
            
            Logger.Debug($"Setting {this}'s stack size to {item.Count}");

            if (count <= 0)
            {
                ctx.InventoryItems.Remove(item);

                // Disassemble item.
                if (LegoDataDictionary.FromString(item.ExtraInfo).TryGetValue("assemblyPartLOTs", out var list))
                {
                    foreach (var part in (LegoDataList) list)
                    {
                        Task.Run(() => Inventory.ManagerComponent.AddItemAsync((int) part, 1));
                    }
                }

                Destroy(this);
            }

            ctx.SaveChanges();
        }

        private void AddCount(uint count)
        {
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

            var message = new AddItemToInventoryMessage
            {
                Associate = Player,
                Item = this,
                ItemLot = Lot,
                Delta = (uint) (count - item.Count),
                Slot = (int) Slot,
                InventoryType = (int) Inventory.InventoryType,
                ShowFlyingLoot = count != default,
                TotalItems = count,
                ExtraInfo = null // TODO
            };
                
            Player.Message(message);
        }

        private void RemoveCount(uint count)
        {
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.First(i => i.InventoryItemId == ObjectId);

            var message = new RemoveItemToInventoryMessage
            {
                Associate = Player,
                Confirmed = true,
                DeleteItem = true,
                OutSuccess = false,
                ItemType = (ItemType) (ItemComponent.ItemType ?? -1),
                InventoryType = Inventory.InventoryType,
                ExtraInfo = null, // TODO
                ForceDeletion = true,
                Item = this,
                Delta = (uint) Math.Abs(count - item.Count),
                TotalItems = count
            };

            Player.Message(message);
        }

        private async Task RemoveFromInventoryAsync()
        {
            await using var ctx = new UchuContext();
            
            var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == ObjectId);

            item.Character.Items.Remove(item);

            await ctx.SaveChangesAsync();
        }
    }
}