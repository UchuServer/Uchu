using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class BouncerComponent : ReplicaComponent
    {
        public bool PetRequired { get; set; }
        
        public Vector3 BouncerDestination { get; set; }

        public override ComponentId Id => ComponentId.BouncerComponent;

        protected BouncerComponent()
        {
            Listen(OnStart, () =>
            {
                if (GameObject.Settings.TryGetValue("bouncer_destination", out var value))
                {
                    BouncerDestination = (Vector3) value;
                }
            });
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);
            writer.WriteBit(!PetRequired);
        }
    }
}