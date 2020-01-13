using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SimplePhysicsComponent : ReplicaComponent
    {
        public bool HasPosition { get; set; } = true;
        
        public bool HasVelocity { get; set; }
        
        public bool HasAirSpeed { get; set; }
        
        public Vector3 LinearVelocity { get; set; }
        
        public Vector3 AngularVelocity { get; set; }
        
        public uint AirSpeed { get; set; }

        public override ComponentId Id => ComponentId.SimplePhysicsComponent;

        public override void Construct(BitWriter writer)
        {
            writer.WriteBit(false);
            writer.Write(0);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (writer.Flag(HasVelocity))
            {
                writer.Write(LinearVelocity);
                writer.Write(AngularVelocity);
            }

            if (writer.Flag(HasAirSpeed))
            {
                writer.Write(AirSpeed);
            }

            if (!writer.Flag(HasPosition)) return;
            
            writer.Write(Transform.Position);
            writer.Write(Transform.Rotation);
        }
    }
}