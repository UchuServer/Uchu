using InfectedRose.Triggers;
using RakDotNet.IO;
using Uchu.World.Client;

namespace Uchu.World
{
    public class TriggerComponent : ReplicaComponent
    {
        public Trigger Trigger { get; set; }

        public override ComponentId Id => ComponentId.TriggerComponent;

        protected TriggerComponent()
        {
            Listen(OnStart, () =>
            {
                if (!GameObject.Settings.TryGetValue("trigger_id", out var triggerIds)) return;

                var str = (string) triggerIds;
                var split = str.Split(':');

                if (split.Length != 2) return;
                
                var triggerPrimaryId = int.Parse(split[0]);
                var triggerId = int.Parse(split[1]);

                foreach (var trigger in Zone.ZoneInfo.Triggers)
                {
                    // TODO: Primary id
                    if (trigger.Id != triggerPrimaryId || trigger.Id != triggerId) continue;
                    
                    Trigger = trigger;
                    
                    break;
                }
            });
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