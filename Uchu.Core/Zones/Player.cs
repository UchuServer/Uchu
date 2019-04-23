using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class Player
    {
        public readonly IPEndPoint EndPoint;
        public readonly Server Server;

        public Player(Server server, IPEndPoint endPoint)
        {
            Server = server;
            EndPoint = endPoint;

            var session = server.SessionCache.GetSession(endPoint);

            CharacterId = session.CharacterId;
            World = server.Worlds[(ZoneId) session.ZoneId];
        }

        public World World { get; set; }
        public long CharacterId { get; set; }
        
        public bool IsBuilding { get; set; }

        public ReplicaPacket ReplicaPacket => World.GetObject(CharacterId);

        public async Task UnequipItemAsync(long objectId)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var item = character.Items.Find(i => i.InventoryItemId == objectId);

                if (item == null)
                    return;

                item.IsEquipped = false;

                await ctx.SaveChangesAsync();

                var obj = World.GetObject(CharacterId);
                var comp = (InventoryComponent) obj.Components.First(c => c is InventoryComponent);

                var items = comp.Items.ToList();

                items.Remove(items.Find(i => i.LOT == item.LOT));

                comp.Items = items.ToArray();

                World.UpdateObject(obj);
            }
        }

        public async Task EquipItemAsync(long objectId)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var item = character.Items.Find(i => i.InventoryItemId == objectId);

                if (item == null)
                    return;
                
                var itemComp = await Server.CDClient.GetItemComponentAsync(
                    (int) await Server.CDClient.GetComponentIdAsync(item.LOT, 11));

                if ((itemComp.ItemType == (int) ItemType.Brick || itemComp.ItemType == (int) ItemType.Model ||
                     itemComp.ItemType == (int) ItemType.Vehicle || itemComp.ItemType == (int) ItemType.LootModel ||
                     item.LOT == 6086) && !IsBuilding)
                {
                    return;
                }

                /*
                 * Unequip already equipped item in that slot.
                 */

                if (character.Items.Any(c => c.IsEquipped))
                    foreach (var inventoryItem in character.Items.Where(c => c.IsEquipped))
                    {
                        var equippedComp =
                            await Server.CDClient.GetItemComponentAsync(
                                (int) await Server.CDClient.GetComponentIdAsync(inventoryItem.LOT, 11));
                        if (equippedComp.ItemType == itemComp.ItemType)
                            await UnequipItemAsync(inventoryItem.InventoryItemId);
                    }

                if (itemComp.IsBoundOnEquip && !item.IsBound) item.IsBound = true;

                item.IsEquipped = true;

                await ctx.SaveChangesAsync();

                var obj = World.GetObject(CharacterId);
                var comp = (InventoryComponent) obj.Components.First(c => c is InventoryComponent);

                var items = comp.Items.ToList();

                items.Add(item);

                comp.Items = items.ToArray();

                World.UpdateObject(obj);
            }
        }

        public async Task ChangeCurrencyAsync(int count)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.FirstAsync(c => c.CharacterId == CharacterId);
                character.Currency += count;

                Server.Send(new SetCurrencyMessage
                {
                    ObjectId = CharacterId,
                    Currency = character.Currency
                }, EndPoint);

                await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        ///     Change the amount of a LOT in the player inventory.
        /// </summary>
        /// <param name="lot">LOT count to change.</param>
        /// <param name="count">The amount of items to add (can be negative)</param>
        /// <param name="forceOnClient">
        ///     Force the removal of these items on the client. Set to false if you know the
        ///     client will remove the item from its inventory itself.
        /// </param>
        /// <returns></returns>
        public async Task ChangeLOTStackAsync(int lot, int count = 1, bool forceOnClient = true)
        {
            if (count > 0)
            {
                await AddItemAsync(lot, count);
                return;
            }
            
            Console.WriteLine($"Changing {lot} with {count}");
            
            using (var ctx = new UchuContext())
            {
                var comp = await Server.CDClient.GetComponentIdAsync(lot, 11);
                var itemComp = await Server.CDClient.GetItemComponentAsync((int) comp);
                
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var list = new List<InventoryItem>();
                
                var item = character.Items.FirstOrDefault(i => i.LOT == lot && i.Count < itemComp.StackSize) ??
                           character.Items.First(i => i.LOT == lot);

                if (item == null) return;

                list.Add(item);
                list.AddRange(character.Items.Where(i => i != item && i.LOT == lot));

                count = Math.Abs(count);
                foreach (var inventoryItem in list)
                {
                    var take = (int) Math.Min(count, inventoryItem.Count);
                    count -= take;
                    Console.WriteLine($"Taking {take} from [{inventoryItem.InventoryItemId}] {inventoryItem.Slot}");
                    if (forceOnClient)
                    {
                        await ChangeItemStackAsync(inventoryItem.InventoryItemId, -take);
                    }
                    else
                    {
                        inventoryItem.Count -= take;
                        if (inventoryItem.Count <= 0)
                        {
                            await DisassembleItemAsync(inventoryItem.InventoryItemId);
                            ctx.InventoryItems.Remove(inventoryItem);
                        }

                        await ctx.SaveChangesAsync();
                    }
                    if (count == 0) return;
                }
            }
        }

        public async Task AddItemAsync(int lot, int count = 1, LegoDataDictionary extraInfo = null,
            InventoryType inventoryType = InventoryType.Invalid)
        {
            var comp = await Server.CDClient.GetComponentIdAsync(lot, 11);
            var itemComp = await Server.CDClient.GetItemComponentAsync((int) comp);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                if (inventoryType == InventoryType.Invalid)
                {
                    try
                    {
                        inventoryType = Utils.GetItemInventoryType((ItemType) itemComp.ItemType);
                    }
                    catch
                    {
                        if (Enum.IsDefined(typeof(PickupLOT), lot))
                            await StatPickup(lot);
                        // TODO: Check for more passable pickup types.
                        return;
                    }
                }

                var items = character.Items.Where(i => i.InventoryType == (int) inventoryType).ToArray();

                /*
                 * Check for already present stack
                 */

                if (items.Any(i => i.LOT == lot) && itemComp.StackSize > 1)
                {
                    if (itemComp.ItemType == (int) ItemType.Brick)
                    {
                        await ChangeItemStackAsync(items.First(i => i.LOT == lot).InventoryItemId, count);
                        return;
                    }

                    if (itemComp.StackSize != 0)
                    {
                        var stack = items.FirstOrDefault(i => i.Count != itemComp.StackSize && i.LOT == lot);

                        if (stack != null)
                        {
                            var left = (int) (stack.Count + count) - itemComp.StackSize;
                            int toAdd;
                            if (left <= 0)
                                toAdd = count;
                            else
                                toAdd = (int) (itemComp.StackSize - stack.Count);
                            await ChangeItemStackAsync(stack.InventoryItemId, toAdd);
                            count -= toAdd;
                        }
                    }
                }

                /*
                 * Create new stack
                 */

                if (count == 0) return;

                var slot = 0;

                if (items.Length > 0 && items.Any(i => i.Slot == 0))
                    for (var j = 0; j < items.Length + 1; j++)
                    {
                        if (items.All(i => i.Slot != j)) break;
                        slot++;
                    }

                var leftToSend = 0;
                if (count - itemComp.StackSize > 0)
                {
                    leftToSend = count - itemComp.StackSize;
                    count = itemComp.StackSize;
                }

                var id = Utils.GenerateObjectId();

                var item = new InventoryItem
                {
                    InventoryItemId = id,
                    LOT = lot,
                    Slot = slot,
                    Count = count == 0 ? 1 : count,
                    InventoryType = (int) inventoryType,
                    IsBound = itemComp.IsBoundOnPickup
                };

                if (extraInfo != null)
                    item.ExtraInfo = extraInfo.ToString();

                character.Items.Add(item);

                await ctx.SaveChangesAsync();

                Server.Send(new AddItemToInventoryMessage
                {
                    ObjectId = CharacterId,
                    ItemLOT = lot,
                    ItemCount = (uint) item.Count,
                    ItemObjectId = id,
                    Slot = item.Slot,
                    InventoryType = (int) inventoryType,
                    ExtraInfo = extraInfo,
                    ShowFlyingLoot = true,
                    IsBound = item.IsBound
                }, EndPoint);

                await UpdateTaskAsync(lot, MissionTaskType.ObtainItem);

                if (leftToSend > 0 && itemComp.StackSize != 0)
                    await AddItemAsync(lot, leftToSend, extraInfo);
            }
        }

        /// <summary>
        ///     Change the number of items in a stack in this Player's inventory. Removes item if nothing is left in the
        ///     stack.
        /// </summary>
        /// <param name="itemId">Item ID</param>
        /// <param name="count">The amount of items to add (can be negative)</param>
        /// <returns></returns>
        public async Task ChangeItemStackAsync(long itemId, int count = 1)
        {
            using (var ctx = new UchuContext())
            {
                InventoryItem itemStack;
                try
                {
                    itemStack = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == itemId);
                }
                catch
                {
                    // It's on the client but removed from the database.
                    return;
                }

                itemStack.Count += count;
                
                if (count > 0)
                {
                    Server.Send(new AddItemToInventoryMessage
                    {
                        ObjectId = CharacterId,
                        ItemObjectId = itemStack.InventoryItemId,
                        ItemLOT = itemStack.LOT,
                        ItemCount = (uint) count,
                        Slot = itemStack.Slot,
                        InventoryType = itemStack.InventoryType,
                        ShowFlyingLoot = count > 0,
                        TotalItems = (uint) itemStack.Count
                    }, EndPoint);
                }
                else if (count < 0)
                {
                    var comp = await Server.CDClient.GetComponentIdAsync(itemStack.LOT, 11);
                    var itemComp = await Server.CDClient.GetItemComponentAsync((int) comp);
                    
                    /*
                     * Remove from player inventory.
                     */
                    
                    Server.Send(new RemoveItemFromInventoryMessageServer
                    {
                        ObjectId = CharacterId,
                        Confirmed = true,
                        DeleteItem = true,
                        OutSuccess = false,
                        ItemType = (ItemType) itemComp.ItemType,
                        InventoryType = (InventoryType) itemStack.InventoryType,
                        ExtraInfo = null,
                        ForceDeletion = true,
                        ObjID = itemStack.InventoryItemId,
                        StackCount = (uint) Math.Abs(count),
                        StackRemaining = (uint) itemStack.Count
                    }, EndPoint);
                }

                if (itemStack.Count == 0)
                {
                    await DisassembleItemAsync(itemId);
                    ctx.InventoryItems.Remove(itemStack);
                }

                await ctx.SaveChangesAsync();
            }
        }

        public async Task DisassembleItemAsync(long itemId)
        {
            using (var ctx = new UchuContext())
            {
                var item = await ctx.InventoryItems.FirstAsync(i => i.InventoryItemId == itemId);
                if (string.IsNullOrEmpty(item.ExtraInfo)) return;

                switch (item.LOT)
                {
                    case 6416:
                        foreach (var part in (LegoDataList) LegoDataDictionary.FromString(item.ExtraInfo)[
                            "assemblyPartLOTs"]) await AddItemAsync((int) part);
                        break;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        ///     Pickup of a stat pickup.
        /// </summary>
        /// <param name="lot"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public async Task StatPickup(int lot)
        {
            Console.WriteLine($"Updating stat for LOT: {lot}");
            var imaginationToAdd = 0;
            var armorToAdd = 0;
            var healthToAdd = 0;

            switch ((PickupLOT) lot)
            {
                case PickupLOT.Imagination:
                    imaginationToAdd = 1;
                    break;
                case PickupLOT.TwoImagination:
                    imaginationToAdd = 2;
                    break;
                case PickupLOT.ThreeImagination:
                    imaginationToAdd = 3;
                    break;
                case PickupLOT.FiveImagination:
                    imaginationToAdd = 5;
                    break;
                case PickupLOT.TenImagination:
                    imaginationToAdd = 10;
                    break;
                case PickupLOT.Health:
                    healthToAdd = 1;
                    break;
                case PickupLOT.TwoHealth:
                    healthToAdd = 2;
                    break;
                case PickupLOT.ThreeHealth:
                    healthToAdd = 3;
                    break;
                case PickupLOT.FiveHealth:
                    healthToAdd = 5;
                    break;
                case PickupLOT.TenHealth:
                    healthToAdd = 10;
                    break;
                case PickupLOT.Armor:
                    armorToAdd = 1;
                    break;
                case PickupLOT.TwoArmor:
                    armorToAdd = 2;
                    break;
                case PickupLOT.ThreeArmor:
                    armorToAdd = 3;
                    break;
                case PickupLOT.FiveArmor:
                    armorToAdd = 5;
                    break;
                case PickupLOT.TenArmor:
                    armorToAdd = 10;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(lot), lot, null);
            }

            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.First(c => c.CharacterId == CharacterId);
                character.CurrentImagination += imaginationToAdd;
                character.CurrentHealth += healthToAdd;
                character.CurrentArmor += armorToAdd;

                await ctx.SaveChangesAsync();
            }

            Console.WriteLine($"Adding: {imaginationToAdd} | {healthToAdd} | {armorToAdd}");

            await UpdateStats();
        }

        public async Task UpdateStats()
        {
            using (var ctx = new UchuContext())
            {
                var character = ctx.Characters.First(c => c.CharacterId == CharacterId);
                var obj = World.GetObject(CharacterId);
                var stats = (StatsComponent) obj.Components.First(c => c is StatsComponent);

                if (character.CurrentImagination > character.MaximumImagination)
                    character.CurrentImagination = character.MaximumImagination;
                if (character.CurrentHealth > character.MaximumHealth)
                    character.CurrentHealth = character.MaximumHealth;
                if (character.CurrentArmor > character.MaximumArmor)
                    character.CurrentArmor = character.MaximumArmor;

                stats.CurrentImagination = (uint) character.CurrentImagination;
                stats.CurrentHealth = (uint) character.CurrentHealth;
                stats.CurrentArmor = (uint) character.CurrentArmor;

                stats.MaxImagination = character.MaximumImagination;
                stats.MaxHealth = character.MaximumHealth;
                stats.MaxArmor = character.MaximumArmor;

                /*
                 * Set correct level
                 */
                
                var level = 0u;
                for (var i = 0u; i < 45; i++)
                {
                    var uScoreRequirement = await Server.CDClient.GetUScoreRequirement(i);
                    if (character.UniverseScore >= uScoreRequirement) continue;
                    level = i - 1;
                    break;
                }

                character.Level = level;

                World.UpdateObject(obj);

                await ctx.SaveChangesAsync();
            }
        }

        public static async Task MoveItemAsync(long item, ulong slot)
        {
            Console.WriteLine($"Moving {item} to {slot}.");
            using (var ctx = new UchuContext())
            {
                ctx.InventoryItems.First(i => i.InventoryItemId == item).Slot = (int) slot;
                await ctx.SaveChangesAsync();
            }
        }

        public async Task UpdateTaskAsync(int id, MissionTaskType type = MissionTaskType.None)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(t => t.Tasks)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                foreach (var mission in character.Missions)
                {
                    if (mission.State != (int) MissionState.Active &&
                        mission.State != (int) MissionState.CompletedActive)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(
                        t => t.Targets.Contains(id) && mission.Tasks.Exists(a => a.TaskId == t.UId));

                    if (task == null)
                        continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.UId);

                    if (!charTask.Values.Contains(id))
                        charTask.Values.Add(id);

                    Server.Send(new NotifyMissionTaskMessage
                    {
                        ObjectId = CharacterId,
                        MissionId = task.MissionId,
                        TaskIndex = tasks.IndexOf(task),
                        Updates = new[] {(float) charTask.Values.Count}
                    }, EndPoint);

                    await ctx.SaveChangesAsync();
                }

                var otherTasks = await Server.CDClient.GetMissionTasksWithTargetAsync(id);

                foreach (var task in otherTasks)
                {
                    var mission = await Server.CDClient.GetMissionAsync(task.MissionId);

                    if (mission.OffererObjectId != -1 || mission.TargetObjectId != -1 || mission.IsMission ||
                        task.TaskType != (int) type)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    if (!character.Missions.Exists(m => m.MissionId == mission.MissionId))
                    {
                        var canOffer = true;

                        foreach (var mId in mission.PrerequiredMissions)
                        {
                            if (!character.Missions.Exists(m => m.MissionId == mId))
                            {
                                canOffer = false;
                                break;
                            }

                            var chrMission = character.Missions.Find(m => m.MissionId == mId);

                            if (!await AllTasksCompletedAsync(chrMission))
                            {
                                canOffer = false;
                                break;
                            }
                        }

                        if (!canOffer)
                            continue;

                        character.Missions.Add(new Mission
                        {
                            MissionId = mission.MissionId,
                            State = (int) MissionState.Active,
                            Tasks = tasks.Select(t => new MissionTask
                            {
                                TaskId = t.UId,
                                Values = new List<float>()
                            }).ToList()
                        });
                    }

                    var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                    if (charMission.State != (int) MissionState.Active ||
                        charMission.State != (int) MissionState.CompletedActive)
                        continue;

                    var charTask = charMission.Tasks.Find(t => t.TaskId == task.UId);

                    if (!charTask.Values.Contains(id))
                        charTask.Values.Add(id);

                    await ctx.SaveChangesAsync();

                    Server.Send(new NotifyMissionTaskMessage
                    {
                        ObjectId = CharacterId,
                        MissionId = mission.MissionId,
                        TaskIndex = tasks.IndexOf(task),
                        Updates = new[] {(float) charTask.Values.Count}
                    }, EndPoint);

                    if (await AllTasksCompletedAsync(charMission))
                        await CompleteMissionAsync(mission);
                }
            }
        }

        public async Task UpdateObjectTaskAsync(MissionTaskType type, long objectId)
        {
            var obj = World.GetObject(objectId);

            if (obj == null)
                return;

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(t => t.Tasks)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                foreach (var mission in character.Missions)
                {
                    if (mission.State != (int) MissionState.Active &&
                        mission.State != (int) MissionState.CompletedActive)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(t =>
                        t.Targets.Contains(obj.LOT) && mission.Tasks.Exists(a => a.TaskId == t.UId));

                    if (task == null)
                        continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.UId);

                    switch (type)
                    {
                        case MissionTaskType.Interact:
                            if (!charTask.Values.Contains(obj.LOT))
                                charTask.Values.Add(obj.LOT);

                            Server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = CharacterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) charTask.Values.Count}
                            }, EndPoint);

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.Collect:
                            var component = (CollectibleComponent) obj.Components.First(c => c is CollectibleComponent);

                            if (!charTask.Values.Contains(component.CollectibleId))
                                charTask.Values.Add(component.CollectibleId);

                            Server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = CharacterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) (component.CollectibleId + (World.ZoneId << 8))}
                            }, EndPoint);

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.KillEnemy:
                            break;
                        case MissionTaskType.Script:
                            if (!charTask.Values.Contains(obj.LOT))
                                charTask.Values.Add(obj.LOT);

                            Server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = CharacterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) charTask.Values.Count}
                            }, EndPoint);

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.QuickBuild:
                            break;
                        case MissionTaskType.GoToNPC:
                            break;
                        case MissionTaskType.UseEmote:
                            break;
                        case MissionTaskType.UseConsumable:
                            break;
                        case MissionTaskType.UseSkill:
                            break;
                        case MissionTaskType.ObtainItem:
                            break;
                        case MissionTaskType.Discover:
                            break;
                        case MissionTaskType.None:
                            break;
                        case MissionTaskType.MinigameAchievement:
                            break;
                        case MissionTaskType.MissionComplete:
                            break;
                        case MissionTaskType.TamePet:
                            break;
                        case MissionTaskType.Racing:
                            break;
                        case MissionTaskType.Flag:
                            break;
                        case MissionTaskType.NexusTowerBrickDonation:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(type), type, null);
                    }
                }

                var otherTasks = await Server.CDClient.GetMissionTasksWithTargetAsync(obj.LOT);

                foreach (var task in otherTasks)
                {
                    var mission = await Server.CDClient.GetMissionAsync(task.MissionId);

                    if (mission.OffererObjectId != -1 || mission.TargetObjectId != -1 || mission.IsMission ||
                        task.TaskType != (int) type)
                        continue;

                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    if (!character.Missions.Exists(m => m.MissionId == mission.MissionId))
                    {
                        var canOffer = true;

                        foreach (var id in mission.PrerequiredMissions)
                        {
                            if (!character.Missions.Exists(m => m.MissionId == id))
                            {
                                canOffer = false;
                                break;
                            }

                            var chrMission = character.Missions.Find(m => m.MissionId == id);

                            if (!await AllTasksCompletedAsync(chrMission))
                            {
                                canOffer = false;
                                break;
                            }
                        }

                        if (!canOffer)
                            continue;

                        character.Missions.Add(new Mission
                        {
                            MissionId = mission.MissionId,
                            State = (int) MissionState.Active,
                            Tasks = tasks.Select(t => new MissionTask
                            {
                                TaskId = t.UId,
                                Values = new List<float>()
                            }).ToList()
                        });
                    }

                    var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                    if (charMission.State != (int) MissionState.Active ||
                        charMission.State != (int) MissionState.CompletedActive)
                        continue;

                    var charTask = charMission.Tasks.Find(t => t.TaskId == task.UId);

                    if (!charTask.Values.Contains(obj.LOT))
                        charTask.Values.Add(obj.LOT);

                    await ctx.SaveChangesAsync();

                    Server.Send(new NotifyMissionTaskMessage
                    {
                        ObjectId = CharacterId,
                        MissionId = mission.MissionId,
                        TaskIndex = tasks.IndexOf(task),
                        Updates = new[] {(float) charTask.Values.Count}
                    }, EndPoint);

                    if (await AllTasksCompletedAsync(charMission))
                        await CompleteMissionAsync(mission);
                }
            }
        }

        public async Task CompleteMissionAsync(MissionsRow mission)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items).Include(c => c.Missions)
                    .ThenInclude(m => m.Tasks).SingleAsync(c => c.CharacterId == CharacterId);

                if (!character.Missions.Exists(m => m.MissionId == mission.MissionId))
                {
                    var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    character.Missions.Add(new Mission
                    {
                        MissionId = mission.MissionId,
                        State = (int) MissionState.Active,
                        Tasks = tasks.Select(t => new MissionTask
                        {
                            TaskId = t.UId,
                            Values = t.Targets.Where(tgt => tgt is int).Select(tgt => (float) (int) tgt).ToList()
                        }).ToList()
                    });
                }

                var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                Server.Send(new NotifyMissionMessage
                {
                    ObjectId = CharacterId,
                    MissionId = mission.MissionId,
                    MissionState = MissionState.Unavailable,
                    SendingRewards = true
                }, EndPoint);

                charMission.State = (int) MissionState.Completed;
                charMission.CompletionCount++;
                charMission.LastCompletion = DateTimeOffset.Now.ToUnixTimeSeconds();

                if (character.MaximumImagination == 0 && mission.MaximumImaginationReward > 0)
                {
                    // Bob mission
                    await CompleteMissionAsync(await Server.CDClient.GetMissionAsync(664));
                    Server.Send(new RestoreToPostLoadStatsMessage {ObjectId = CharacterId}, EndPoint);
                }

                character.Currency += mission.CurrencyReward;
                character.UniverseScore += mission.LegoScoreReward;
                character.MaximumHealth += mission.MaximumHealthReward;
                character.MaximumImagination += mission.MaximumImaginationReward;

                if (mission.CurrencyReward > 0)
                    Server.Send(new SetCurrencyMessage
                    {
                        ObjectId = CharacterId,
                        Currency = character.Currency,
                        Position = Vector3.Zero // TODO: find out what to set this to
                    }, EndPoint);

                if (mission.LegoScoreReward > 0)
                    Server.Send(new ModifyLegoScoreMessage
                    {
                        ObjectId = CharacterId,
                        SourceType = 2,
                        Score = mission.LegoScoreReward
                    }, EndPoint);

                if (mission.MaximumImaginationReward > 0)
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["amount"] = character.MaximumImagination.ToString(),
                        ["type"] = "imagination"
                    };

                    Server.Send(new UIMessageToClientMessage
                    {
                        ObjectId = CharacterId,
                        Arguments = new AMF3<object>(dict),
                        MessageName = "MaxPlayerBarUpdate"
                    }, EndPoint);
                }

                if (mission.MaximumHealthReward > 0)
                {
                    var dict = new Dictionary<string, object>
                    {
                        ["amount"] = character.MaximumHealth.ToString(),
                        ["type"] = "health"
                    };

                    Server.Send(new UIMessageToClientMessage
                    {
                        ObjectId = CharacterId,
                        Arguments = new AMF3<object>(dict),
                        MessageName = "MaxPlayerBarUpdate"
                    }, EndPoint);
                }

                if (mission.FirstItemReward != -1)
                    await AddItemAsync(mission.FirstItemReward, mission.FirstItemRewardCount);

                if (mission.SecondItemReward != -1)
                    await AddItemAsync(mission.SecondItemReward, mission.SecondItemRewardCount);

                if (mission.ThirdItemReward != -1)
                    await AddItemAsync(mission.ThirdItemReward, mission.ThirdItemRewardCount);

                if (mission.FourthItemReward != -1)
                    await AddItemAsync(mission.FourthItemReward, mission.FourthItemRewardCount);

                Server.Send(new NotifyMissionMessage
                {
                    ObjectId = CharacterId,
                    MissionId = mission.MissionId,
                    MissionState = MissionState.Completed,
                    SendingRewards = false
                }, EndPoint);

                await UpdateTaskAsync(mission.MissionId, MissionTaskType.MissionComplete);

                await ctx.SaveChangesAsync();
            }
        }

        public async Task<bool> AllTasksCompletedAsync(Mission mission)
        {
            var tasks = await Server.CDClient.GetMissionTasksAsync(mission.MissionId);

            foreach (var task in tasks)
                Console.WriteLine(
                    $"TASK {mission.Tasks.Find(t => t.TaskId == task.UId).Values.Count} | {task.TargetValue}");

            return tasks.TrueForAll(t => mission.Tasks.Find(t2 => t2.TaskId == t.UId).Values.Count >= t.TargetValue);
        }

        public async Task OfferMissionAsync(long offererId)
        {
            var obj = World.GetObject(offererId);

            if (obj == null)
                return;

            var componentId = await Server.CDClient.GetComponentIdAsync(obj.LOT, 73);
            var missions = await Server.CDClient.GetNPCMissionsAsync((int) componentId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                foreach (var mission in missions)
                {
                    var miss = await Server.CDClient.GetMissionAsync(mission.MissionId);

                    if (!miss.IsMission)
                        continue;

                    if (mission.AcceptsMission)
                        if (character.Missions.Exists(m => m.MissionId == mission.MissionId))
                        {
                            var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                            if (charMission.State != (int) MissionState.Completed &&
                                await AllTasksCompletedAsync(charMission))
                            {
                                Server.Send(new OfferMissionMessage
                                {
                                    ObjectId = character.CharacterId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = offererId
                                }, EndPoint);

                                /*_server.Send(new OfferMissionMessage
                                {
                                    ObjectId = character.CharacterId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = offererId
                                }, _endpoint);*/

                                break;
                            }
                        }

                    if (mission.OffersMission)
                        if (!character.Missions.Exists(m => m.MissionId == mission.MissionId) ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.Active ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.ReadyToComplete)
                        {
                            var canOffer = true;

                            foreach (var id in miss.PrerequiredMissions)
                            {
                                if (!character.Missions.Exists(m => m.MissionId == id))
                                {
                                    canOffer = false;
                                    break;
                                }

                                var chrMission = character.Missions.Find(m => m.MissionId == id);

                                if (!await AllTasksCompletedAsync(chrMission))
                                {
                                    canOffer = false;
                                    break;
                                }
                            }

                            if (!canOffer)
                                continue;

                            Server.Send(new OfferMissionMessage
                            {
                                ObjectId = character.CharacterId,
                                MissionId = mission.MissionId,
                                OffererObjectId = offererId
                            }, EndPoint);

                            /*_server.Send(new OfferMissionMessage
                            {
                                ObjectId = character.CharacterId,
                                MissionId = mission.MissionId,
                                OffererObjectId = offererId
                            }, _endpoint);*/

                            break;
                        }
                }
            }
        }

        public async Task LaunchRocket(long objectId)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var rocket =
                    character.Items.Find(i => i.LOT == 6416); // TODO: find out how to properly get the active rocket

                Server.Send(new EquipItemMessage
                {
                    ObjectId = CharacterId,
                    ItemObjectId = rocket.InventoryItemId
                }, EndPoint);

                Server.Send(new ChangeObjectWorldStateMessage
                {
                    ObjectId = rocket.InventoryItemId,
                    State = ObjectWorldState.Attached
                }, EndPoint);

                Server.Send(new FireClientEventMessage
                {
                    ObjectId = objectId,
                    Arguments = "RocketEquipped",
                    TargetObjectId = rocket.InventoryItemId,
                    SenderObjectId = CharacterId
                }, EndPoint);

                character.LandingByRocket = true;
                character.Rocket =
                    ((LegoDataList) LegoDataDictionary.FromString(rocket.ExtraInfo)["assemblyPartLOTs"]).ToString(";") +
                    ";";

                await ctx.SaveChangesAsync();
            }
        }

        public async Task Smash()
        {
            Console.WriteLine("Smashing player...");
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                Server.Send(new DieMessage
                {
                    ClientDeath = true,
                    DeathType = "electro-shock-death",
                    SpawnLoot = false,
                    LootOwner = CharacterId
                }, EndPoint);
            }
        }
    }
}