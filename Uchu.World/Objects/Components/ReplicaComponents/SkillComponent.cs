using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Systems.Behaviors;

namespace Uchu.World
{
    public class SkillComponent : ReplicaComponent
    {
        private Dictionary<BehaviorSlot, uint> ActiveBehaviors { get; }
        
        private Dictionary<uint, ExecutionContext> HandledSkills { get; }
        
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

        private uint BehaviorSyncIndex { get; set; }
        
        public SkillEntry[] DefaultSkillSet { get; private set; }

        public Lot SelectedConsumeable { get; set; }

        public uint SelectedSkill
        {
            get => ActiveBehaviors[BehaviorSlot.Primary];
            set => ActiveBehaviors[BehaviorSlot.Primary] = value;
        }
        
        public override ComponentId Id => ComponentId.SkillComponent;

        protected SkillComponent()
        {
            ActiveBehaviors = new Dictionary<BehaviorSlot, uint>();
            HandledSkills = new Dictionary<uint, ExecutionContext>();
            
            Listen(OnStart, async () =>
            {
                await using var cdClient = new CdClientContext();

                var skills = await cdClient.ObjectSkillsTable.Where(
                    s => s.ObjectTemplate == GameObject.Lot
                ).ToArrayAsync();

                DefaultSkillSet = skills
                    .Where(s => s.SkillID != default)
                    .Select(s => new SkillEntry
                    {
                        SkillId = (uint) (s.SkillID ?? 0),
                        Type = (SkillCastType) (s.CastOnType ?? 0),
                        AiCombatWeight = s.AICombatWeight ?? 0
                    })
                    .ToArray();

                SetupStandardSkills();
                
                if (!(GameObject is Player))
                    return;

                ActiveBehaviors[BehaviorSlot.Primary] = 1;
            });
        }

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }

        private void SetupStandardSkills()
        {
            foreach (var skillEntry in DefaultSkillSet)
            {
                Logger.Debug($"{GameObject} has skill [{skillEntry.Type}] {skillEntry.SkillId}");
                
                if (skillEntry.Type != SkillCastType.OnSpawn) continue;

                CalculateSkill((int) skillEntry.SkillId);

                if (!GameObject.TryGetComponent<DestructibleComponent>(out var destructibleComponent)) continue;

                Listen(destructibleComponent.OnResurrect, () =>
                {
                    CalculateSkill((int) skillEntry.SkillId);
                });
            }
        }

        public async Task MountItemAsync(Lot item)
        {
            if (item == default) return;
            
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();

            RemoveSkill(slot);
            
            await MountSkill(item);

            await EquipSkill(item);
        }

        public async Task DismountItemAsync(Lot item)
        {
            if (item == default) return;

            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();
            
            RemoveSkill(slot);

            DismountSkill(item);
        }

        private async Task EquipSkill(Lot item)
        {
            if (item == default) return;
            
            await using var ctx = new CdClientContext();

            var itemInfo = await ctx.ItemComponentTable.FirstOrDefaultAsync(
                i => i.Id == item.GetComponentId(ComponentId.ItemComponent)
            );
            
            if (itemInfo == default) return;

            var slot = ((ItemType) (itemInfo.ItemType ?? 0)).GetBehaviorSlot();
            
            var infos = BehaviorTree.GetSkillsForObject(item);

            var onUse = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnUse);

            if (onUse == default) return;
            
            RemoveSkill(slot);
            
            SetSkill(slot, (uint) onUse.SkillId);
        }

        private async Task MountSkill(Lot item)
        {
            if (item == default)
                return;
            
            var infos = BehaviorTree.GetSkillsForObject(item);
            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);
            
            if (onEquip == default)
                return;
            
            var tree = BehaviorTree.FromLot(item);
            tree.Deserialize(GameObject, new BitReader(new MemoryStream()));
            tree.Mount();

            if (GameObject.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
            {
                await missionInventory.UseSkillAsync(onEquip.SkillId);
            }
        }

        private void DismountSkill(Lot item)
        {
            if (item == default)
                return;
            
            var infos = BehaviorTree.GetSkillsForObject(item);
            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);

            if (onEquip == default)
                return;
            
            var tree = BehaviorTree.FromLot(item);
            tree.Deserialize(GameObject, new BitReader(new MemoryStream()));
            tree.Dismantle();
        }

        /// <summary>
        /// Calculates a skill by serializing it
        /// </summary>
        /// <param name="skillId">The skill Id to serialize</param>
        /// <param name="precalculate">Precalculate the skill, exits execution but adds behavior to cache</param>
        /// <returns>The skill time in milliseconds</returns>
        public float CalculateSkill(int skillId, bool precalculate = false)
        {
            var tree = BehaviorTree.FromSkill(skillId);
            if (precalculate)
                return 0;

            var stream = new MemoryStream();
            using var writer = new BitWriter(stream, leaveOpen: true);
            var syncId = ClaimSyncId();

            var context = tree.Serialize(GameObject, writer, skillId, syncId, Transform.Position);
            if (!context.FoundTarget)
                return 0;

            Zone.BroadcastMessage(new EchoStartSkillMessage
            {
                Associate = GameObject,
                CastType = 0,
                Content = stream.ToArray(),
                SkillId = skillId,
                SkillHandle = syncId,
                OptionalOriginator = GameObject,
                OriginatorRotation = GameObject.Transform.Rotation
            });

            tree.Execute();
            return context.SkillTime * 1000;
        }

        public float CalculateSkill(int skillId, GameObject target)
        {
            var tree = BehaviorTree.FromSkill(skillId);

            var stream = new MemoryStream();
            using var writer = new BitWriter(stream, leaveOpen: true);

            var syncId = ClaimSyncId();
            var context = tree.Serialize(GameObject, writer, skillId, syncId, Transform.Position, target);

            Zone.BroadcastMessage(new EchoStartSkillMessage
            {
                Associate = GameObject,
                CastType = 0,
                Content = stream.ToArray(),
                SkillId = skillId,
                SkillHandle = syncId,
                OptionalOriginator = GameObject,
                OriginatorRotation = GameObject.Transform.Rotation
            });

            tree.Execute();

            return context.SkillTime;
        }

        public async Task StartUserSkillAsync(StartSkillMessage message)
        {
            if (!(GameObject is Player)) return;
            
            try
            {
                if (message.OptionalTarget != null)
                {
                    // There should be more to this
                    if (!message.OptionalTarget.Alive)
                        message.OptionalTarget = null;
                    else if (!message.OptionalTarget.GetComponent<DestructibleComponent>()?.Alive ?? true)
                        message.OptionalTarget = null;
                    /* else if (Vector3.Distance(message.OptionalTarget.Transform.Position, Transform.Position) > TargetRange)
                        message.OptionalTarget = null;*/
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return;
            }

            var stream = new MemoryStream(message.Content);
            using (var reader = new BitReader(stream, leaveOpen: true))
            {
                var tree = BehaviorTree.FromSkill(message.SkillId);
                var context = tree.Deserialize(
                    GameObject,
                    reader,
                    SkillCastType.OnUse,
                    message.OptionalTarget
                );
                
                HandledSkills[message.SkillHandle] = context;

                Zone.ExcludingMessage(new EchoStartSkillMessage
                {
                    Associate = GameObject,
                    CasterLatency = message.CasterLatency,
                    CastType = message.CastType,
                    Content = message.Content,
                    LastClickedPosition = message.LastClickedPosition,
                    OptionalOriginator = message.OptionalOriginator,
                    OptionalTarget = message.OptionalTarget,
                    OriginatorRotation = message.OriginatorRotation,
                    SkillHandle = message.SkillHandle,
                    SkillId = message.SkillId,
                    UsedMouse = message.UsedMouse
                }, GameObject as Player);

                tree.Execute();
                
                if (GameObject.TryGetComponent<DestroyableComponent>(out var stats))
                {
                    var info = tree.BehaviorIds.FirstOrDefault(b => b.SkillId == message.SkillId);
                    if (info != default)
                    {
                        stats.Imagination = (uint) ((int) stats.Imagination - info.ImaginationCost);
                    }
                }
            }
            
            await GameObject.GetComponent<MissionInventoryComponent>().UseSkillAsync(
                message.SkillId
            );
        }

        public void SyncUserSkillAsync(SyncSkillMessage message)
        {
            var stream = new MemoryStream(message.Content);
            using var reader = new BitReader(stream, leaveOpen: true);

            var found = HandledSkills.TryGetValue(message.SkillHandle, out var behavior);
            
            Logger.Debug($"SYNC: {message.SkillHandle} [{message.BehaviorHandle}] ; {found}");

            if (found)
            {
                behavior.SyncAsync(message.BehaviorHandle, reader);
            }

            if (message.Done)
            {
                //_handledSkills.Remove(message.SkillHandle);
            }

            Zone.ExcludingMessage(new EchoSyncSkillMessage
            {
                Associate = GameObject,
                BehaviorHandle = message.BehaviorHandle,
                Content = message.Content,
                Done = message.Done,
                SkillHandle = message.SkillHandle
            }, GameObject as Player);
        }

        private void SetSkill(BehaviorSlot slot, uint skillId)
        {
            if (!(GameObject is Player player)) return;

            if (ActiveBehaviors.TryGetValue(slot, out var currentSkill))
            {
                ActiveBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                player.Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }

            ActiveBehaviors[slot] = skillId;

            Logger.Information($"Selected skill: [{slot}]: {skillId}");
            
            player.Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }
        
        public void RemoveSkill(BehaviorSlot slot)
        {
            if (!(GameObject is Player player)) return;

            if (ActiveBehaviors.TryGetValue(slot, out var currentSkill))
            {
                ActiveBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                player.Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }
            
            // Get default skill
            var skillId = slot switch
            {
                BehaviorSlot.Invalid => 0u,
                BehaviorSlot.None => 0u,
                BehaviorSlot.Head => 0u,
                BehaviorSlot.LeftHand => 0u,
                BehaviorSlot.Consumeable => 0u,
                BehaviorSlot.Neck => 0u,
                BehaviorSlot.Primary => 1u,
                _ => throw new ArgumentOutOfRangeException(nameof(slot), slot, null)
            };

            ActiveBehaviors[slot] = skillId;
            
            Logger.Information($"Selected default skill: [{slot}]: {skillId}");
            
            player.Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }

        public uint ClaimSyncId()
        {
            lock (this)
            {
                return ++BehaviorSyncIndex;
            }
        }
    }
}