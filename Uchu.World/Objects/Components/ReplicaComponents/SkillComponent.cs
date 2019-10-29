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
        
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

        public readonly Dictionary<uint, ExecutionContext> HandledSkills = new Dictionary<uint, ExecutionContext>();

        public Lot SelectedConsumeable { get; set; }

        public uint SelectedSkill
        {
            get => _activeBehaviors[BehaviorSlot.Primary];
            set => _activeBehaviors[BehaviorSlot.Primary] = value;
        }
        
        public override ComponentId Id => ComponentId.SkillComponent;

        public SkillComponent()
        {
            OnStart.AddListener(() =>
            {
                if (!(GameObject is Player)) return;
                
                if (!GameObject.TryGetComponent<InventoryComponent>(out var inventory)) return;

                _activeBehaviors.Add(BehaviorSlot.Primary, 1);
                
                inventory.OnEquipped.AddListener(async item =>
                {
                    if (item == default) return;
                    
                    var infos = await BehaviorTree.GetSkillsForItem(item);

                    var onUse = infos.FirstOrDefault(i => i.CastType == SkillCastType.OnUse);

                    if (onUse == default) return;
                    
                    As<Player>().SendChatMessage($"Adding skill: {onUse.SkillId}");
                    
                    SetSkill(item.ItemType.GetBehaviorSlot(), (uint) onUse.SkillId);
                });
                
                inventory.OnUnEquipped.AddListener(item =>
                {
                    if (item == default) return Task.CompletedTask;
                    
                    RemoveSkill(item.ItemType.GetBehaviorSlot());
                    
                    return Task.CompletedTask;
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

                var context = await tree.ExecuteAsync(GameObject, reader, SkillCastType.OnUse);

                HandledSkills[message.SkillHandle] = context;
            }

            await As<Player>().GetComponent<QuestInventory>().UpdateObjectTaskAsync(
                MissionTaskType.UseSkill, message.SkillId
            );
        }

        public async Task SyncUserSkillAsync(SyncSkillMessage message)
        {
            As<Player>().SendChatMessage($"SYNC: {message.SkillHandle} [{message.BehaviourHandle}]");
            
            var stream = new MemoryStream(message.Content);
            using var reader = new BitReader(stream, leaveOpen: true);

            await HandledSkills[message.SkillHandle].SyncAsync(message.BehaviourHandle, reader);

            if (message.Done)
            {
                HandledSkills.Remove(message.SkillHandle);
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