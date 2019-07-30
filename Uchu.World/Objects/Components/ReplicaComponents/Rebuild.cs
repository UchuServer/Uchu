using System.Collections.Generic;
using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [Essential]
    [RequireComponent(typeof(Stats), true)]
    public class Rebuild : ScriptedActivity
    {
        public RebuildState State { get; set; } = RebuildState.Open;
        
        public bool Success { get; set; }
        
        public bool Enabled { get; set; }
        
        public float TimeSinceStart { get; set; }
        
        public float PauseTime { get; set; }
        
        public Vector3 ActivatorPosition { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Rebuild;
        
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);

            writer.WriteBit(false);
            writer.Write(ActivatorPosition);
            writer.WriteBit(false);
        }

        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);
            
            writer.WriteBit(true);
            writer.Write((uint) State);
            writer.WriteBit(Success);
            writer.WriteBit(Enabled);
            writer.Write(TimeSinceStart);
            writer.Write(PauseTime);
        }
    }
}