using RakDotNet.IO;

namespace Uchu.World
{
    public class VehiclePhysicsComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.VehiclePhysicsComponent;

        public byte ControlScheme { get; set; } = 5;
        
        public override void Construct(BitWriter writer)
        {
            writer.Write(ControlScheme);
            writer.WriteBit(true);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(false);
        }
    }
}