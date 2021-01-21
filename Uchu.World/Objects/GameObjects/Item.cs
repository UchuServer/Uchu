using System;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    [Unconstructed]
    public class Item : GameObject
    {
        public bool IsPackage { get; private set; }
        
        public bool IsConsumable { get; private set; }
        
        protected Item()
        {
            OnConsumed = new Event();

            Listen(OnStart, () =>
            {
                IsPackage = Lot.GetComponentId(ComponentId.PackageComponent) != default;

                var skills = ClientCache.GetTable<ObjectSkills>().Where(
                    s => s.ObjectTemplate == Lot
                ).ToArray();

                IsConsumable = skills.Any(
                    s => s.CastOnType == (int) SkillCastType.OnConsumed
                );
            });
            
            Listen(OnDestroyed, () => Inventory.UnManageItem(this));
        }

        public ItemComponent ItemComponent
        {
            get
            {
                var id = Lot.GetComponentId(ComponentId.ItemComponent);

                return ClientCache.GetTable<ItemComponent>().FirstOrDefault(c => c.Id == id);
            }
        }
        
        public Event OnConsumed { get; }

        public Inventory Inventory { get; private set; }

        public Player Player { get; private set; }

        public uint Count
        {
            get
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.Id == Id);

                return (uint) info.Count;
            }
            set
            {
                UpdateCountAsync(value).Wait();
                
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.FirstOrDefault(i => i.Id == Id);

                if (info == default) return;
                
                info.Count = value;
                
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
                
                var info = ctx.InventoryItems.First(i => i.Id == Id);

                return (uint) info.Slot;
            }
            set
            {
                using var ctx = new UchuContext();
                
                var info = ctx.InventoryItems.First(i => i.Id == Id);

                info.Slot = (int) value;
                
                ctx.SaveChanges();
            }
        }

        public ItemType ItemType => (ItemType) (ItemComponent.ItemType ?? (int) ItemType.Invalid);

        public async Task EquipAsync(bool skipAllChecks = false)
        {
            if (ItemComponent.IsBOE ?? false)
            {
                await BindAsync();
            }

            var inventory = Player.GetComponent<InventoryComponent>();

            await inventory.EquipItemAsync(this, skipAllChecks);
        }

        public async Task UnEquipAsync()
        {
            var inventory = Player.GetComponent<InventoryComponent>();

            await inventory.UnEquipItemAsync(this);
        }

        public async Task<bool> IsEquippedAsync()
        {
            var item = await Id.FindItemAsync();

            return item.IsEquipped;
        }

        public async Task<bool> IsBoundAsync()
        {
            var item = await Id.FindItemAsync();

            return item.IsBound;
        }

        public async Task BindAsync()
        {
            await using var ctx = new UchuContext();

            var item = await ctx.InventoryItems.FirstAsync(i => i.Id == Id);

            item.IsBound = true;

            await ctx.SaveChangesAsync();
        }

        public async Task UseNonEquipmentItem()
        {
            if (!IsPackage) return;
            
            await OnConsumed.InvokeAsync();
            
            var container = AddComponent<LootContainerComponent>();

            await container.CollectDetailsAsync();
            
            await Inventory.ManagerComponent.RemoveItemAsync(Lot, 1);
            
            var manager = Inventory.ManagerComponent;
            
            foreach (var item in await container.GenerateLootYieldsAsync(Player))
            {
                await manager.AddItemAsync(item.Key, item.Value);
            }
        }
        
        public async Task ConsumeAsync()
        {
            await Player.GetComponent<MissionInventoryComponent>().UseConsumableAsync(Lot);
            
            if (!IsConsumable) return;
            
            await Inventory.ManagerComponent.RemoveItemAsync(Lot, 1);
        }

        public static Item Instantiate(long itemId, Inventory inventory)
        {
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.FirstOrDefault(
                i => i.Id == itemId && i.Character.Id == inventory.ManagerComponent.GameObject.Id
            );

            if (item == default)
            {
                Logger.Error($"{itemId} is not an item on {inventory.ManagerComponent.GameObject}");
                return null;
            }

            var cdClientObject = ClientCache.GetTable<Objects>().FirstOrDefault(
                o => o.Id == item.Lot
            );

            var itemRegistryEntry = ((Lot) item.Lot).GetComponentId(ComponentId.ItemComponent);

            if (cdClientObject == default || itemRegistryEntry == default)
            {
                Logger.Error($"{itemId} [{item.Lot}] is not a valid item");
                return null;
            }

            var instance = Instantiate<Item>
            (
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: itemId, lot: item.Lot
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
            using var ctx = new UchuContext();
            
            var cdClientObject = ClientCache.GetTable<Objects>().FirstOrDefault(
                o => o.Id == lot
            );

            var itemRegistryEntry = ClientCache.GetTable<ComponentsRegistry>().FirstOrDefault(
                r => r.Id == lot && r.Componenttype == 11
            );

            if (cdClientObject == default || itemRegistryEntry == default)
            {
                Logger.Error($"<new item> [{lot}] is not a valid item");
                return null;
            }

            var instance = Instantiate<Item>
            (
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: ObjectId.Standalone, lot: lot
            );

            instance.Settings = extraInfo ?? new LegoDataDictionary();

            var itemComponent = ClientCache.GetTable<ItemComponent>().First(
                i => i.Id == itemRegistryEntry.Componentid
            );

            instance.Inventory = inventory;
            instance.Player = inventory.ManagerComponent.GameObject as Player;

            var playerCharacter = ctx.Characters.Include(c => c.Items).First(
                c => c.Id == inventory.ManagerComponent.GameObject.Id
            );

            var inventoryItem = new InventoryItem
            {
                Count = count,
                InventoryType = (int) inventory.InventoryType,
                Id = instance.Id,
                IsBound = itemComponent.IsBOP ?? false,
                Slot = (int) slot,
                Lot = lot,
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

            var item = await ctx.InventoryItems.FirstAsync(i => i.Id == Id);
            
            item.Count = count;
            
            Logger.Debug($"Setting {this}'s stack size to {item.Count}");

            if (count <= 0)
            {
                if (await IsEquippedAsync())
                    await UnEquipAsync();
                
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
        
        private async Task UpdateCountAsync(uint count)
        {
            if (count >= Count)
            {
                AddCount(count);
            }
            else
            {
                RemoveCount(count);
            }

            if (count > 0) return;
            
            await using var ctx = new UchuContext();

            var item = await ctx.InventoryItems.FirstOrDefaultAsync(
                i => i.Id == Id
            );

            if (item == default) return;

            if (await IsEquippedAsync())
                await UnEquipAsync();

            ctx.InventoryItems.Remove(item);
            
            await ctx.SaveChangesAsync();

            // Disassemble item.
            if (LegoDataDictionary.FromString(item.ExtraInfo).TryGetValue("assemblyPartLOTs", out var list))
            {
                foreach (var part in (LegoDataList) list)
                {
                    await Inventory.ManagerComponent.AddItemAsync((int) part, 1);
                }
            }

            Destroy(this);
        }

        private void AddCount(uint count)
        {
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.First(i => i.Id == Id);

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
            
            var item = ctx.InventoryItems.First(i => i.Id == Id);

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
            
            var item = await ctx.InventoryItems.FirstAsync(i => i.Id == Id);

            item.Character.Items.Remove(item);

            await ctx.SaveChangesAsync();
        }
    }
}