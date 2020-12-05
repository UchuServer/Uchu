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
        public Item()
        {
            OnConsumed = new Event();

            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
            
            Listen(OnDestroyed, () => Inventory.UnManageItem(this));
        }

        public static Item Instantiate(Lot lot, ObjectId parentObjectId = ObjectId.Standalone)
        {
            var instance = Instantiate<Item>
            (
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: ObjectId.Standalone, lot: lot
            );
            
            var instance = Instantiate<Item>
            (
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: itemId, lot: item.Lot
            );
        }

        public async Task LoadAsync(CdClientContext clientContext, UchuContext uchuContext)
        {
            IsPackage = Lot.GetComponentId(ComponentId.PackageComponent) != default;
            
            var skills = clientContext.ObjectSkillsTable.Where(
                s => s.ObjectTemplate == Lot
            ).ToArray();

            IsConsumable = skills.Any(
                s => s.CastOnType == (int) SkillCastType.OnConsumed
            );

            var id = Lot.GetComponentId(ComponentId.ItemComponent);
            ItemComponent = clientContext.ItemComponentTable.FirstOrDefault(c => c.Id == id);
            
            var info = uchuContext.InventoryItems.First(i => i.Id == Id);
            
            Count = (uint) info.Count;
            Slot = (uint) info.Slot;

            ParentId = info.ParentId;
            IsEquipped = info.IsEquipped;
            IsBound = info.IsBound;
        }

        public ItemComponent ItemComponent { get; private set; }
        public Event OnConsumed { get; }
        public Inventory Inventory { get; private set; }
        public Player Player { get; private set; }
        public uint Count { get; set; }
        public bool IsEquipped { get; set; }
        public bool IsBound { get; private set; }
        public bool IsPackage { get; private set; }
        public bool IsConsumable { get; private set; }
        public ObjectId ParentId { get; private set; }

        /// <summary>
        /// Returns the lots of the sub items of this item
        /// </summary>
        public Lot[] SubItemLots => !string.IsNullOrWhiteSpace(ItemComponent.SubItems)
            ? ItemComponent.SubItems.Replace(" ", "").Split(',')
                .Select(i => (Lot) int.Parse(i)).ToArray()
            : new Lot[] { };

        /// <summary>
        ///     The slot this item inhabits.
        /// </summary>
        /// <remarks>
        ///     Should only be set as a response to a client request.
        /// </remarks>
        public uint Slot { get; private set; }

        public ItemType ItemType => (ItemType) (ItemComponent.ItemType ?? (int) ItemType.Invalid);

        public async Task EquipAsync(bool skipAllChecks = false)
        {
            if (ItemComponent.IsBOE ?? false)
            {
                IsBound = true;
            }

            var inventory = Player.GetComponent<InventoryComponent>();
            await inventory.EquipItemAsync(this, skipAllChecks);
        }

        public async Task UnEquipAsync()
        {
            var inventory = Player.GetComponent<InventoryComponent>();
            await inventory.UnEquipItemAsync(this);
        }

        public async Task UseNonEquipmentItem()
        {
            if (!IsPackage) return;
            
            await OnConsumed.InvokeAsync();
            
            var container = AddComponent<LootContainerComponent>();

            await container.CollectDetailsAsync();
            
            await Inventory.ManagerComponent.RemoveItemAsync(Lot, 1);
            
            var manager = Inventory.ManagerComponent;
            
            foreach (var lot in await container.GenerateLootYieldsAsync(Player))
            {
                await manager.AddItemAsync(lot, 1);
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
            using var cdClient = new CdClientContext();
            using var ctx = new UchuContext();
            
            var item = ctx.InventoryItems.FirstOrDefault(
                i => i.Id == itemId && i.Character.Id == inventory.ManagerComponent.GameObject.Id
            );

            if (item == default)
            {
                Logger.Error($"{itemId} is not an item on {inventory.ManagerComponent.GameObject}");
                return null;
            }

            var cdClientObject = cdClient.ObjectsTable.FirstOrDefault(
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
            using var cdClient = new CdClientContext();
            
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
                inventory.ManagerComponent.Zone, cdClientObject.Name, objectId: ObjectId.Standalone, lot: lot
            );

            instance.Settings = extraInfo ?? new LegoDataDictionary();

            var itemComponent = cdClient.ItemComponentTable.First(
                i => i.Id == itemRegistryEntry.Componentid
            );

            instance.Inventory = inventory;
            instance.Player = inventory.ManagerComponent.GameObject as Player;
            
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
                IsBound = instance.IsBound,
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
                if (IsEquipped)
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
                MessageAddItem(count);
            }
            else
            {
                MessageRemoveItem(count);
            }

            Count = count;

            if (count > 0)
                return;

            if (IsEquipped)
                await UnEquipAsync();

            // Disassemble item.
            if (Settings.TryGetValue("assemblyPartLOTs", out var list))
            {
                foreach (var part in (LegoDataList) list)
                {
                    await Inventory.ManagerComponent.AddItemAsync((int) part, 1);
                }
            }

            Destroy(this);
        }

        private void MessageAddItem(uint count)
        {
            Player.Message(new AddItemToInventoryMessage
            {
                Associate = Player,
                Item = this,
                ItemLot = Lot,
                Delta = count - Count,
                Slot = (int) Slot,
                InventoryType = (int) Inventory.InventoryType,
                ShowFlyingLoot = count != default,
                TotalItems = count,
                ExtraInfo = null // TODO
            });
        }

        private void MessageRemoveItem(uint count)
        {
            Player.Message(new RemoveItemToInventoryMessage
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
                Delta = (uint) Math.Abs(count - Count),
                TotalItems = count
            });
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