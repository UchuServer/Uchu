using System.Numerics;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    [RequireComponent(typeof(StatsComponent), true)]
    public class RebuildComponent : ScriptedActivityComponent
    {
        public RebuildState State { get; set; } = RebuildState.Open;
        
        public bool Success { get; set; }
        
        public bool Enabled { get; set; }
        
        public float TimeSinceStart { get; set; }
        
        public float PauseTime { get; set; }
        
        public Vector3 ActivatorPosition { get; set; }
        
        public override ReplicaComponentsId Id => ReplicaComponentsId.Rebuild;

        public override void FromLevelObject(LevelObject levelObject)
        {
            ActivatorPosition = (Vector3) levelObject.Settings["rebuild_activators"];
        }

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