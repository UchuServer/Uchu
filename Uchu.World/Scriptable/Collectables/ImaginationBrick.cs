using System;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    [AutoAssign(lot:6726)]
    public class ImaginationBrick : GameScript
    {
        public ImaginationBrick(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        public override void Start()
        {
            Console.WriteLine($"Flag: [{LOT}] {ObjectID}");
            foreach (var (key, value) in ReplicaPacket.Settings)
            {
                Console.WriteLine($"[{key}] : {value}");
            }
        }
    }
}