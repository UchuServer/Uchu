using System;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class TriggerComponent : ReplicaComponent
    {
        public Trigger Trigger { get; private set; }
        
        public override ComponentId Id => ComponentId.TriggerComponent;

        protected TriggerComponent()
        {
            Listen(OnStart, () =>
            {
                if (!GameObject.Settings.TryGetValue("trigger_id", out var triggerIds)) return;

                Logger.Information($"{GameObject} Attempting trigger: {triggerIds}");
                
                var str = (string) triggerIds;
                var split = str.Split(':');

                if (split.Length != 2)
                {
                    Logger.Error($"{GameObject} Failed to parse trigger: {triggerIds}");
                    
                    return;
                }
                
                var fileId = int.Parse(split[0]);
                var triggerId = int.Parse(split[1]);

                LoadTrigger(fileId, triggerId);
            });
        }

        public void LoadTrigger(int fileId, int triggerId)
        {
            var trigger = Zone.ZoneInfo.TriggerDictionary[fileId, triggerId];
                
            if (trigger == default)
            {
                trigger = Zone.ZoneInfo.TriggerDictionary[triggerId, fileId];

                if (trigger == default)
                {
                    Logger.Error($"{GameObject} Failed to find trigger: {triggerId}:{fileId}");

                    return;
                }
            }
                
            if (trigger.Enabled == 0) return;

            LoadTrigger(trigger);
        }

        public void LoadTrigger(Trigger trigger)
        {
            Trigger = trigger;
            
            Logger.Information($"{GameObject} Trigger: {Trigger.FileId}:{Trigger.Id}");

            foreach (var @event in trigger.Events)
            {
                PhysicsComponent physics;
                switch (@event.Id)
                {
                    case "OnCreate":
                        ExecuteEvent(@event);
                        break;
                    case "OnDestroy":
                        Listen(OnDestroyed, () =>
                        {
                            ExecuteEvent(@event);
                        });
                        break;
                    case "OnEnter":
                        physics = GameObject.AddComponent<PhysicsComponent>();
                            
                        Listen(physics.OnEnter, other =>
                        {
                            Logger.Information($"Enter: {other.GameObject}");

                            ExecuteEvent(@event, other.GameObject);
                        });
                        break;;
                    case "OnExit":
                        physics = GameObject.AddComponent<PhysicsComponent>();
                            
                        Listen(physics.OnLeave, other =>
                        {
                            Logger.Information($"Left: {other.GameObject}");
                                
                            ExecuteEvent(@event, other.GameObject);
                        });
                        break;
                    default:
                        Logger.Error($"Unsupported event type: {@event.Id}!");
                        break;
                }
            }
        }

        private void ExecuteEvent(TriggerEvent @event, params object[] arguments)
        {
            foreach (var command in @event.Commands)
            {
                ExecuteTriggerCommand(command, arguments);
            }
        }

        private void ExecuteTriggerCommand(TriggerCommand command, params object[] arguments)
        {
            Logger.Information($"TRIGGER: {command.Id} -> {string.Join(", ", arguments)}");
            
            switch (command.Id)
            {
                case "SetPhysicsVolumeEffect":
                    SetPhysicsVolumeEffect(command);
                    break;
                case "CastSkill":
                    CastSkill(command, arguments);
                    break;
                case "pushObject":
                    PushObject(command, arguments);
                    break;
                case "repelObject":
                    RepealObject(command, arguments);
                    break;
            }

            GameObject.Serialize(GameObject);
        }

        private void PushObject(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = new Vector3
                {
                    X = float.Parse(parameters[0]),
                    Y = float.Parse(parameters[1]),
                    Z = float.Parse(parameters[2])
                }
            });
        }

        private void RepealObject(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;

            var targetDirection = Transform.Position - target.Transform.Position;

            var rotation = targetDirection.QuaternionLookRotation(Vector3.UnitY);

            var forward = rotation.VectorMultiply(Vector3.UnitX);

            var parameters = command.Arguments.Split(',');
            
            target.SendChatMessage($"Knockback!");
            
            target.Message(new KnockbackMessage
            {
                Associate = target,
                Caster = GameObject,
                Originator = GameObject,
                KnockbackTime = 1,
                Vector = forward * float.Parse(parameters[0])
            });
        }

        private void CastSkill(TriggerCommand command, params object[] arguments)
        {
            if (!(arguments[0] is Player target)) return;
            
            target.SendChatMessage($"Laser!");
            
            var parameters = command.Arguments.Split(',');

            var skill = int.Parse(parameters[0]);

            var skillComponent = GameObject.AddComponent<SkillComponent>();

            var _ = Task.Run(async () =>
            {
                try
                {
                    await skillComponent.CalculateSkillAsync(skill, target);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }

        private void SetPhysicsVolumeEffect(TriggerCommand command)
        {
            if (!GameObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) return;

            var arguments = command.Arguments.Split(',');
                    
            physicsComponent.IsEffectActive = true;

            var effectTypeInfo = typeof(PhantomPhysicsEffectType);

            var effectType = (PhantomPhysicsEffectType) Enum.Parse(effectTypeInfo, arguments[0]);

            physicsComponent.EffectType = effectType;

            var amount = float.Parse(arguments[1]);

            physicsComponent.EffectAmount = amount;

            if (arguments.Length > 2)
            {
                var direction = new Vector3
                {
                    X = float.Parse(arguments[2]),
                    Y = float.Parse(arguments[3]),
                    Z = float.Parse(arguments[4])
                };

                physicsComponent.EffectDirection = direction;
            }
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            var hasId = Trigger != default;

            writer.WriteBit(hasId);

            if (hasId) writer.Write(Trigger.Id);
        }
    }
}