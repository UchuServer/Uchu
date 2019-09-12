using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        public readonly Dictionary<uint, Behavior> HandledBehaviors = new Dictionary<uint, Behavior>();
        public readonly Dictionary<uint, Behavior> HandledSkills = new Dictionary<uint, Behavior>();
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Skill;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
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
            if (Player == null) return;
            
            using (var cdClient = new CdClientContext())
            {
                var behavior = await cdClient.SkillBehaviorTable.FirstOrDefaultAsync(
                    s => s.SkillID == message.SkillId
                );

                if (behavior?.BehaviorID == default)
                {
                    Logger.Error($"{GameObject} is trying to use an invalid skill {message.SkillId}");
                    return;
                }

                var template = await Behavior.GetTemplate(behavior.BehaviorID ?? 0);
                
                var stream = new MemoryStream(message.Content);
                
                var executioner = new BehaviorExecutioner
                {
                    Executioner = Player
                };
                
                using (var reader = new BitReader(stream))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");
                    
                    Logger.Debug($"Starting behaviour {(BehaviorTemplateId) template.TemplateID}: Target = {message.OptionalTarget}");

                    if (message.OptionalTarget != null)
                        executioner.Targets.Add(message.OptionalTarget);
                    
                    var instance = (Behavior) Activator.CreateInstance(
                        Behavior.Behaviors[(BehaviorTemplateId) template.TemplateID]
                    );

                    HandledSkills.Add(message.SkillHandle, instance);
                    
                    instance.Executioner = executioner;
                    instance.BehaviorId = (int) behavior.BehaviorID;
                    instance.SkillComponent = this;
                    
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
                    
                    await instance.Serialize(reader);
                }

                await Player.GetComponent<QuestInventory>().UpdateObjectTaskAsync(
                    MissionTaskType.UseSkill, message.SkillId
                );
            }
        }

        public async Task SyncUserSkillAsync(SyncSkillMessage message)
        {
            Zone.BroadcastMessage(new EchoSyncSkillMessage
            {
                Associate = GameObject,
                BehaviorHandle = message.BehaviourHandle,
                Content = message.Content,
                Done = message.Done,
                SkillHandle = message.SkillHandle
            });

            if (HandledBehaviors.TryGetValue(message.BehaviourHandle, out var head))
            {
                Logger.Debug($"Syncing skill Done = {message.Done}, Handle = {message.BehaviourHandle}");

                if (message.Done)
                {
                    head.Executioner.Execute();
                    HandledBehaviors.Remove(message.BehaviourHandle);
                }

                var template = await Behavior.GetTemplate(head.BehaviorId);

                if (message.Content.Length == default) return;

                var stream = new MemoryStream(message.Content);

                using (var reader = new BitReader(stream))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");

                    Logger.Debug($"Syncing behaviour {(BehaviorTemplateId) template.TemplateID}...");

                    await head.StartBranch(head.BehaviorId, reader);
                }
            }
            else if (HandledSkills.TryGetValue(message.SkillHandle, out head))
            {
                Logger.Debug($"Syncing skill Done = {message.Done}, Handle = {message.SkillHandle}");

                if (message.Done)
                {
                    head.Executioner.Execute();
                    HandledBehaviors.Remove(message.SkillHandle);
                }

                var template = await Behavior.GetTemplate(head.BehaviorId);

                if (message.Content.Length == default) return;

                var stream = new MemoryStream(message.Content);

                using (var reader = new BitReader(stream))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");

                    Logger.Debug($"Syncing behaviour {(BehaviorTemplateId) template.TemplateID}...");

                    await head.StartBranch(head.BehaviorId, reader);
                }
            }
        }
    }
}