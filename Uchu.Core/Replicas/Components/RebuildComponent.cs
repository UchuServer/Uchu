using System.Numerics;
using RakDotNet;

namespace Uchu.Core
{
    public class RebuildComponent : ReplicaComponent
    {
        public RebuildState State { get; set; } = RebuildState.Open;
        public bool Success { get; set; } = false;
        public bool Enabled { get; set; } = true;
        public float TimeSinceStart { get; set; } = 0;
        public float PausedTime { get; set; } = 0;

        public Vector3 ActivatorPosition { get; set; }

        public override void Serialize(BitStream stream)
        {
            stream.WriteBit(true);
            stream.WriteUInt((uint) State);
            stream.WriteBit(Success);
            stream.WriteBit(Enabled);
            stream.WriteFloat(TimeSinceStart);
            stream.WriteFloat(PausedTime);
        }

        public override void Construct(BitStream stream)
        {
            Serialize(stream);

            stream.WriteBit(false);
            stream.WriteFloat(ActivatorPosition.X);
            stream.WriteFloat(ActivatorPosition.Y);
            stream.WriteFloat(ActivatorPosition.Z);
            stream.WriteBit(false);
        }
    }
}