using System;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    [AutoAssign(typeof(TriggerComponent))]
    public class TriggerScript : GameScript
    {
        public TriggerScript(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        public override void Start()
        {
            
        }
    }
}