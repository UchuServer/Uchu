using System;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    [AutoAssign(lot:6604)]
    public class SpawnerScript : GameScript
    {
        public SpawnerScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        public override void Start()
        {
            Console.WriteLine($"\nSpawner Settings: {ObjectID}\n");
            foreach (var (key, value) in ReplicaPacket.Settings)
            {
                Console.WriteLine($"[{key}] : {value}");
            }
        }
    }
}