using System.Collections.Generic;
using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public class ScriptedActivity : ReplicaComponent
    {
        public List<GameObject> Players { get; set; } = new List<GameObject>();

        public float[] Parameters { get; set; } = new float[10];

        public override ReplicaComponentsId Id => ReplicaComponentsId.ScriptedActivity;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.Write((uint) Players.Count);

            foreach (var contributor in Players)
            {
                writer.Write(contributor);

                foreach (var parameter in Parameters)
                {
                    writer.Write(parameter);
                }
            }
        }
    }
}