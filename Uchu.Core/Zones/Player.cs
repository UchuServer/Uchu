using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Collections;

namespace Uchu.Core
{
    public class Player
    {
        private readonly IPEndPoint _endpoint;
        private readonly Server _server;

        public World World { get; set; }
        public long CharacterId { get; set; }

        public Player(Server server, IPEndPoint endpoint)
        {
            _server = server;
            _endpoint = endpoint;

            var session = server.SessionCache.GetSession(endpoint);

            CharacterId = session.CharacterId;
            World = server.Worlds[(ZoneId) session.ZoneId];
        }

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

        public async Task RemoveItemAsync(int lot, int count = 1)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var item = character.Items.Find(i => i.LOT == lot);

                if (item.Count - count > 0)
                {
                    item.Count -= count;
                }
                else
                {
                    character.Items.Remove(item);
                }

                await ctx.SaveChangesAsync();
            }
        }

        public async Task AddItemAsync(int lot, int count = 1, LegoDataDictionary extraInfo = null)
        {
            var comp = await _server.CDClient.GetComponentIdAsync(lot, 11);
            var itemComp = await _server.CDClient.GetItemComponentAsync((int) comp);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var id = Utils.GenerateObjectId();
                var inventoryType = (int) Utils.GetItemInventoryType((ItemType) itemComp.ItemType);

                var items = character.Items.Where(i => i.InventoryType == inventoryType).ToArray();

                var slot = 0;

                if (items.Length > 0)
                {
                    var max = items.Max(i => i.Slot);
                    slot = max + 1;

                    for (var i = 0; i < max; i++)
                    {
                        if (items.All(itm => itm.Slot != i))
                            slot = i;
                    }
                }

                var item = new InventoryItem
                {
                    InventoryItemId = id,
                    LOT = lot,
                    Slot = slot,
                    Count = count,
                    InventoryType = inventoryType
                };

                if (extraInfo != null)
                    item.ExtraInfo = extraInfo.ToString(";");

                character.Items.Add(item);

                await ctx.SaveChangesAsync();

                _server.Send(new AddItemToInventoryMessage
                {
                    ObjectId = CharacterId,
                    ItemLOT = lot,
                    ItemCount = (uint) item.Count,
                    ItemObjectId = id,
                    Slot = item.Slot,
                    InventoryType = inventoryType,
                    ExtraInfo = extraInfo
                }, _endpoint);

                await UpdateTaskAsync(lot);
            }
        }

        public async Task UpdateTaskAsync(int id)
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

                    var tasks = await _server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(
                        t => t.TargetLOTs.Contains(id) && mission.Tasks.Exists(a => a.TaskId == t.UId));

                    if (task == null)
                        continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.UId);

                    if (!charTask.Values.Contains(id))
                        charTask.Values.Add(id);

                    _server.Send(new NotifyMissionTaskMessage
                    {
                        ObjectId = CharacterId,
                        MissionId = task.MissionId,
                        TaskIndex = tasks.IndexOf(task),
                        Updates = new[] {(float) charTask.Values.Count}
                    }, _endpoint);

                    await ctx.SaveChangesAsync();
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

                    var tasks = await _server.CDClient.GetMissionTasksAsync(mission.MissionId);

                    var task = tasks.Find(t =>
                        t.TargetLOTs.Contains(obj.LOT) && mission.Tasks.Exists(a => a.TaskId == t.UId));

                    if (task == null)
                        continue;

                    var charTask = mission.Tasks.Find(t => t.TaskId == task.UId);

                    switch (type)
                    {
                        case MissionTaskType.Interact:
                            if (!charTask.Values.Contains(obj.LOT))
                                charTask.Values.Add(obj.LOT);

                            _server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = CharacterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) charTask.Values.Count}
                            }, _endpoint);

                            await ctx.SaveChangesAsync();
                            break;
                        case MissionTaskType.Collect:
                            var component = (CollectibleComponent) obj.Components.First(c => c is CollectibleComponent);

                            if (!charTask.Values.Contains(component.CollectibleId))
                                charTask.Values.Add(component.CollectibleId);

                            _server.Send(new NotifyMissionTaskMessage
                            {
                                ObjectId = CharacterId,
                                MissionId = task.MissionId,
                                TaskIndex = tasks.IndexOf(task),
                                Updates = new[] {(float) (component.CollectibleId + (World.ZoneId << 8))}
                            }, _endpoint);

                            await ctx.SaveChangesAsync();
                            break;
                    }
                }
            }
        }

        public async Task<bool> AllTasksCompletedAsync(Mission mission)
        {
            var tasks = await _server.CDClient.GetMissionTasksAsync(mission.MissionId);

            return tasks.TrueForAll(t => mission.Tasks.Find(t2 => t2.TaskId == t.UId).Values.Count >= t.TargetValue);
        }

        public async Task OfferMissionAsync(long offererId)
        {
            var obj = World.GetObject(offererId);

            if (obj == null)
                return;

            var componentId = await _server.CDClient.GetComponentIdAsync(obj.LOT, 73);
            var missions = await _server.CDClient.GetNPCMissionsAsync((int) componentId);

            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Missions).ThenInclude(m => m.Tasks)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                foreach (var mission in missions)
                {
                    if (mission.AcceptsMission)
                    {
                        if (character.Missions.Exists(m => m.MissionId == mission.MissionId))
                        {
                            var charMission = character.Missions.Find(m => m.MissionId == mission.MissionId);

                            if (charMission.State != (int) MissionState.Completed && await AllTasksCompletedAsync(charMission))
                            {
                                _server.Send(new OfferMissionMessage
                                {
                                    ObjectId = character.CharacterId,
                                    MissionId = mission.MissionId,
                                    OffererObjectId = offererId
                                }, _endpoint);

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

                    if (mission.OffersMission)
                    {
                        if (!character.Missions.Exists(m => m.MissionId == mission.MissionId) ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.Active ||
                            character.Missions.Find(m => m.MissionId == mission.MissionId).State ==
                            (int) MissionState.ReadyToComplete)
                        {
                            var miss = await _server.CDClient.GetMissionAsync(mission.MissionId);

                            var canOffer = true;

                            foreach (var id in miss.PrerequiredMissions)
                            {
                                if (id == 664)
                                    continue;

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

                            _server.Send(new OfferMissionMessage
                            {
                                ObjectId = character.CharacterId,
                                MissionId = mission.MissionId,
                                OffererObjectId = offererId
                            }, _endpoint);

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
        }

        public async Task LaunchRocket(long objectId)
        {
            using (var ctx = new UchuContext())
            {
                var character = await ctx.Characters.Include(c => c.Items)
                    .SingleAsync(c => c.CharacterId == CharacterId);

                var rocket = character.Items.Find(i => i.LOT == 6416); // TODO: find out how to properly get the active rocket

                _server.Send(new EquipItemMessage
                {
                    ObjectId = CharacterId,
                    ItemObjectId = rocket.InventoryItemId
                }, _endpoint);

                _server.Send(new ChangeObjectWorldStateMessage
                {
                    ObjectId = rocket.InventoryItemId,
                    State = ObjectWorldState.Attached
                }, _endpoint);

                _server.Send(new FireClientEventMessage
                {
                    ObjectId = objectId,
                    Arguments = "RocketEquipped",
                    TargetObjectId = rocket.InventoryItemId,
                    SenderObjectId = CharacterId
                }, _endpoint);
            }
        }
    }
}