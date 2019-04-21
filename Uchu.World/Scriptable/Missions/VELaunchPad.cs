using System;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Scriptable;

namespace Uchu.World.Scriptable
{
    [AutoAssign(lot:6368)]
    public class VELaunchPad : GameScript
    {
        public VELaunchPad(Core.World world, ReplicaPacket replicaPacket) : base(world, replicaPacket)
        {
        }

        public override void Start()
        {
            Console.WriteLine($"{ObjectID} [{LOT}] is a {typeof(VELaunchPad)}");
        }

        public override async Task OnUse(Player player)
        {
             await Task.Run(async () => await player.UpdateTaskAsync(5652, MissionTaskType.Script));
             await Task.Run(async () => await player.UpdateTaskAsync(LOT, MissionTaskType.Script));
             await Task.Run(async () => await player.UpdateObjectTaskAsync(MissionTaskType.Script, ObjectID));
        }
    }
}