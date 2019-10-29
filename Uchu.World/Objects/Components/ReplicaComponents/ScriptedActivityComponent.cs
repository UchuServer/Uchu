using System.Collections.Generic;
using RakDotNet.IO;

namespace Uchu.World
{
    public class ScriptedActivityComponent : ReplicaComponent
    {
        public readonly List<GameObject> Participants = new List<GameObject>();

        public float[] Parameters { get; set; } = new float[10];

        public override ComponentId Id => ComponentId.ScriptedActivityComponent;

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write((uint) Participants.Count);

            foreach (var contributor in Participants)
            {
                writer.Write(contributor);

                foreach (var parameter in Parameters) writer.Write(parameter);
            }
        }
    }
}