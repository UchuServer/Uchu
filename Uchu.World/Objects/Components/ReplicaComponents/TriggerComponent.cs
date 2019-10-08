using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class TriggerComponent : ReplicaComponent
    {
        public int TriggerId { get; set; } = -1;

        public override ReplicaComponentsId Id => ReplicaComponentsId.Trigger;

        public override void FromLevelObject(LevelObject levelObject)
        {
            if (!levelObject.Settings.TryGetValue("trigger_id", out var triggerId)) return;

            var str = (string) triggerId;
            var colonIndex = str.IndexOf(':');
            var v = str.Substring(colonIndex + 1);

            TriggerId = int.Parse(v);

            Logger.Information($"{GameObject} is a trigger [{triggerId}]");
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            var hasId = TriggerId != -1;

            writer.WriteBit(hasId);

            if (hasId) writer.Write(TriggerId);
        }

        public bool CheckCollision(Transform transform) => CheckCollision(transform.Position);

        public bool CheckCollision(Vector3 position)
        {
            return false;
        }
    }
}