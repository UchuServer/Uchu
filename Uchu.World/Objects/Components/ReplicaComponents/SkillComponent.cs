using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.CdClient;
using Uchu.World.Behaviors;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class SkillComponent : ReplicaComponent
    {
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

        public readonly Dictionary<uint, ExecutionContext> HandledSkills = new Dictionary<uint, ExecutionContext>();

        public Lot SelectedConsumeable { get; set; }
        
        public int SelectedSkill { get; set; }
        
        public override ComponentId Id => ComponentId.SkillComponent;

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

                As<Player>().SendChatMessage($"START: {SelectedSkill}: {message.SkillHandle}");
                
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
                Logger.Debug(HandledSkills[message.SkillHandle].DebugGraph());
                
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
    }
}