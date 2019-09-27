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
        /*
         * TODO: Rework
         * 
         * Right now the contents of skill packages are kept in stream which may be accessed later, no they cannot be
         * closed. This has to be fixed.
         */
        
        // This number is taken from testing and is not concrete.
        public const float TargetRange = 11.6f;

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
            if (As<Player>() == null) return;

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

                var executioner = new BehaviorExecutioner
                {
                    Executioner = As<Player>()
                };

                var stream = new MemoryStream(message.Content);
                using (var reader = new BitReader(stream, leaveOpen: true))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");

                    // Remove player check for *tap to die* :P
                    // TODO: Remove, replace with PvP checks
                    if (message.OptionalTarget != null && !(message.OptionalTarget is Player))
                    {
                        var distance = Vector3.Distance(message.OptionalTarget.Transform.Position, Transform.Position);

                        foreach (var gameObject in Zone.GameObjects.Where(g => g.Layer == Layer.Smashable))
                        {
                            if (gameObject == message.OptionalTarget) continue;

                            if (Vector3.Distance(gameObject.Transform.Position, Transform.Position) < distance)
                            {
                                //
                                // Player is closer to another smashable object and should therefore not face this one in game.
                                //
                                // Invalid target.
                                //
                                goto NoFixedTarget;
                            }
                        }

                        if (distance < TargetRange) executioner.Targets.Add(message.OptionalTarget);
                    }

                    //
                    // No fixed target was specified or could not be verified.
                    //
                    NoFixedTarget:

                    var instance = (Behavior) Activator.CreateInstance(
                        Behavior.Behaviors[(BehaviorTemplateId) template.TemplateID]
                    );

                    Logger.Information($"{GameObject} is starting skill {message.SkillHandle}");
                    
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

                    await instance.SerializeAsync(reader);
                }

                await As<Player>().GetComponent<QuestInventory>().UpdateObjectTaskAsync(
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
                Logger.Debug($"Syncing behaviors done = {message.Done}, Handle = {message.BehaviourHandle}");

                if (message.Done)
                {
                    head.Executioner.Execute();
                    HandledBehaviors.Remove(message.BehaviourHandle);
                }

                var template = await Behavior.GetTemplate(head.BehaviorId);

                if (message.Content.Length == default) return;

                var stream = new MemoryStream(message.Content);
                using (var reader = new BitReader(stream, leaveOpen: true))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");

                    Logger.Debug($"Syncing behaviour {(BehaviorTemplateId) template.TemplateID}...");

                    await head.StartBranch(head.BehaviorId, reader);
                }
            }
            
            if (HandledSkills.TryGetValue(message.SkillHandle, out head))
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
                using (var reader = new BitReader(stream, leaveOpen: true))
                {
                    Debug.Assert(template.TemplateID != null, "template.TemplateID != null");

                    Logger.Debug($"Syncing behaviour {(BehaviorTemplateId) template.TemplateID}...");

                    await head.StartBranch(head.BehaviorId, reader);
                }
            }
        }
    }
}