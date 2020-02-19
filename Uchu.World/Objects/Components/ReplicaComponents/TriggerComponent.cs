using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class TriggerComponent : ReplicaComponent
    {
        public Trigger Trigger { get; set; }

        public override ComponentId Id => ComponentId.TriggerComponent;

        protected TriggerComponent()
        {
            Listen(OnStart, async () =>
            {
                if (!GameObject.Settings.TryGetValue("trigger_id", out var triggerIds)) return;

                var str = (string) triggerIds;
                var split = str.Split(':');

                if (split.Length != 2) return;
                
                var fileId = int.Parse(split[0]);
                var triggerId = int.Parse(split[1]);

                Trigger = Zone.ZoneInfo.TriggerDictionary[fileId, triggerId];

                if (Trigger == default)
                {
                    Logger.Error($"Failed to find trigger: {triggerId}:{fileId}");
                    
                    return;
                }
                
                if (Trigger.Enabled == 0) return;

                foreach (var @event in Trigger.Events)
                {
                    Logger.Debug($"TRIGGER EVENT: {@event.Id} -> {@event.Commands.FirstOrDefault()?.Id}");
                    
                    switch (@event.Id)
                    {
                        case "OnCreate":
                            foreach (var command in @event.Commands)
                            {
                                await ExecuteTriggerCommand(command);
                            }
                            
                            break;
                    }
                }
            });
        }

        private async Task ExecuteTriggerCommand(TriggerCommand command)
        {
            switch (command.Id)
            {
                case "SetPhysicsVolumeEffect":
                    if (!GameObject.TryGetComponent<PhantomPhysicsComponent>(out var physicsComponent)) return;

                    var arguments = command.Arguments.Split(',');
                    
                    physicsComponent.IsEffectActive = true;

                    var effectTypeInfo = typeof(PhantomPhysicsEffectType);

                    var effectType = (PhantomPhysicsEffectType) Enum.Parse(effectTypeInfo, arguments[0]);

                    physicsComponent.EffectType = effectType;

                    var amount = int.Parse(arguments[1]);

                    physicsComponent.EffectAmount = amount;

                    if (arguments.Length > 2)
                    {
                        var direction = new Vector3
                        {
                            X = float.Parse(arguments[2]),
                            Y = float.Parse(arguments[2]),
                            Z = float.Parse(arguments[2])
                        };

                        physicsComponent.EffectDirection = direction;
                    }
                    
                    Logger.Information($"PHYSICS: {physicsComponent.EffectType} -> {physicsComponent.EffectType} -> {physicsComponent.EffectDirection}");

                    break;
            }

            GameObject.Serialize(GameObject);
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