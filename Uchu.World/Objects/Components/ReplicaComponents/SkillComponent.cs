using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Behaviors;

namespace Uchu.World
{
    public class SkillComponent : ReplicaComponent
    {
        private readonly Dictionary<BehaviorSlot, uint> _activeBehaviors = new Dictionary<BehaviorSlot, uint>();
        
        private readonly Dictionary<uint, ExecutionContext> _handledSkills = new Dictionary<uint, ExecutionContext>();
        
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

        public Lot SelectedConsumeable { get; set; }

        public uint SelectedSkill
        {
            get => _activeBehaviors[BehaviorSlot.Primary];
            set => _activeBehaviors[BehaviorSlot.Primary] = value;
        }
        
        public override ComponentId Id => ComponentId.SkillComponent;

        protected SkillComponent()
        {
            OnStart.AddListener(() =>
            {
                if (!GameObject.TryGetComponent<InventoryComponent>(out var inventory)) return;

                _activeBehaviors.Add(BehaviorSlot.Primary, 1);
                
                inventory.OnEquipped.AddListener(MountItem);

                inventory.OnUnEquipped.AddListener(DismountItem);

                if (!(GameObject is Player player)) return;
                
                player.OnWorldLoad.AddListener(async () =>
                {
                    if (!GameObject.TryGetComponent<InventoryManagerComponent>(out var manager)) return;
                    
                    foreach (var item in manager[InventoryType.Items].Items.Where(i => i.Equipped))
                    {
                        await MountItem(item);
                    }
                });
            });
        }
        
        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
        }

        private async Task MountItem(Item item)
        {
            if (item == default) return;
            
            await MountSkill(item);

            await EquipSkill(item);
        }

        private async Task DismountItem(Item item)
        {
            if (item == default) return;

            var slot = item.ItemType.GetBehaviorSlot();
            
            RemoveSkill(slot);

            await DismountSkill(item);
        }

        private async Task EquipSkill(Item item)
        {
            if (item == default) return;
            
            var infos = await BehaviorTree.GetSkillsForItem(item);

            var onUse = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnUse);

            if (onUse == default) return;
            
            As<Player>().SendChatMessage($"Adding skill: {onUse.SkillId}");

            var slot = item.ItemType.GetBehaviorSlot();
            
            RemoveSkill(slot);
            
            SetSkill(slot, (uint) onUse.SkillId);
        }

        private async Task MountSkill(Item item)
        {
            if (item == default) return;
            
            var infos = await BehaviorTree.GetSkillsForItem(item);
            
            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);

            if (onEquip == default) return;
            
            As<Player>().SendChatMessage($"Mount skill: {onEquip.SkillId}");
            
            var tree = new BehaviorTree(item.Lot);

            await tree.BuildAsync();

            await tree.MountAsync(GameObject);
        }

        private async Task DismountSkill(Item item)
        {
            if (item == default) return;
            
            var infos = await BehaviorTree.GetSkillsForItem(item);

            var onEquip = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnEquip);

            if (onEquip == default) return;
            
            As<Player>().SendChatMessage($"Dismount skill: {onEquip.SkillId}");
            
            var tree = new BehaviorTree(item.Lot);

            await tree.BuildAsync();

            await tree.DismantleAsync(GameObject);
        }

        public async Task StartUserSkillAsync(StartSkillMessage message)
        {
            if (As<Player>() == null) return;

            var stream = new MemoryStream(message.Content);
            using (var reader = new BitReader(stream, leaveOpen: true))
            {
                Zone.BroadcastMessage(new EchoStartSkillMessage
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
                });

                As<Player>().SendChatMessage($"START: {message.SkillId}");
                
                var tree = new BehaviorTree(message.SkillId);

                await tree.BuildAsync();

                var context = await tree.ExecuteAsync(GameObject, reader, SkillCastType.OnUse, message.OptionalTarget);

                _handledSkills[message.SkillHandle] = context;
            }

            GameObject.GetComponent<MissionInventoryComponent>().UpdateObjectTask(
                MissionTaskType.UseSkill, message.SkillId
            );
        }

        public async Task SyncUserSkillAsync(SyncSkillMessage message)
        {
            As<Player>().SendChatMessage($"SYNC: {message.SkillHandle} [{message.BehaviourHandle}]");
            
            var stream = new MemoryStream(message.Content);
            using var reader = new BitReader(stream, leaveOpen: true);

            await _handledSkills[message.SkillHandle].SyncAsync(message.BehaviourHandle, reader);

            if (message.Done)
            {
                _handledSkills.Remove(message.SkillHandle);
            }

            Zone.BroadcastMessage(new EchoSyncSkillMessage
            {
                Associate = GameObject,
                BehaviorHandle = message.BehaviourHandle,
                Content = message.Content,
                Done = message.Done,
                SkillHandle = message.SkillHandle
            });
        }

        public void SetSkill(BehaviorSlot slot, uint skillId)
        {
            if (_activeBehaviors.TryGetValue(slot, out var currentSkill))
            {
                _activeBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                As<Player>().Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }

            _activeBehaviors[slot] = skillId;

            Logger.Information($"Selected skill: [{slot}]: {skillId}");
            
            As<Player>().Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }
        
        public void RemoveSkill(BehaviorSlot slot)
        {
            if (_activeBehaviors.TryGetValue(slot, out var currentSkill))
            {
                _activeBehaviors.Remove(slot);
                
                Logger.Information($"Removed skill: [{slot}]: {currentSkill}");
                
                As<Player>().Message(new RemoveSkillMessage
                {
                    Associate = GameObject,
                    SkillId = currentSkill
                });
            }

            //
            // Get default skill
            //
            
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

            _activeBehaviors[slot] = skillId;
            
            Logger.Information($"Selected default skill: [{slot}]: {skillId}");
            
            As<Player>().Message(new AddSkillMessage
            {
                Associate = GameObject,
                CastType = SkillCastType.OnUse,
                SlotId = slot,
                SkillId = skillId
            });
        }
    }
}