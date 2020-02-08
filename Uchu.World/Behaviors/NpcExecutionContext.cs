using System.IO;
using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World.Behaviors
{
    public class NpcExecutionContext : ExecutionContext
    {
        public float MinRange { get; set; }
        
        public float MaxRange { get; set; }
        
        public int SkillId { get; set; }
        
        public uint SyncSkillId { get; set; }
        
        public bool FoundTarget { get; set; }
        
        public NpcExecutionContext(GameObject associate, BitWriter writer, int skillId, uint syncSkillId) : base(associate, default, writer)
        {
            SkillId = skillId;
            SyncSkillId = syncSkillId;
        }

        public void Sync(uint behaviorSyncId)
        {
            Associate.Zone.BroadcastMessage(new EchoSyncSkillMessage
            {
                Associate = Associate,
                SkillHandle = SyncSkillId,
                BehaviorHandle = behaviorSyncId,
                Content = (Writer.BaseStream as MemoryStream)?.ToArray(),
                Done = false
            });
        }

        public NpcExecutionContext Copy()
        {
            return new NpcExecutionContext(Associate, new BitWriter(new MemoryStream()), SkillId, SyncSkillId)
            {
                MaxRange = MaxRange,
                MinRange = MinRange
            };
        }

        public bool IsValidTarget(GameObject gameObject)
        {
            var distance = Vector3.Distance(gameObject.Transform.Position, Associate.Transform.Position);

            return distance <= MaxRange;
        }
    }
}