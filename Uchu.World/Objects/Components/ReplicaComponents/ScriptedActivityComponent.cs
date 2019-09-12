using System.Collections.Generic;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class ScriptedActivityComponent : ReplicaComponent
    {
        public List<GameObject> Players { get; set; } = new List<GameObject>();

        public float[] Parameters { get; set; } = new float[10];

        public override ReplicaComponentsId Id => ReplicaComponentsId.ScriptedActivity;

        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

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