using System.Numerics;
using RakDotNet.IO;
using Uchu.World.Parsers;

namespace Uchu.World
{
    public class ShootingGalleryComponent : ScriptedActivityComponent
    {
        public override ReplicaComponentsId Id => ReplicaComponentsId.ShootingGallery;
        
        public Vector3 ActivityCameraPosition { get; set; }
        
        public Vector3 ActivityCameraLookAt { get; set; }
        
        public bool Active { get; set; }
        
        public double CannonVelocity { get; set; }
        
        public double CannonReFireRate { get; set; }
        
        public double CannonMinDistance { get; set; }
        
        public Vector3 CannonBarrelOffset { get; set; }
        
        public float CannonAngle { get; set; }
        
        public Vector3 Facing { get; set; }
        
        public GameObject Operator { get; set; }
        
        public float CannonTimeout { get; set; }
        
        public float CannonFoV { get; set; }
        
        public override void FromLevelObject(LevelObject levelObject)
        {
            
        }

        public override void Construct(BitWriter writer)
        {
            base.Construct(writer);

            writer.Write(ActivityCameraPosition);
            writer.Write(ActivityCameraLookAt);

            SerializeCannon(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            base.Serialize(writer);

            SerializeCannon(writer);
        }

        private void SerializeCannon(BitWriter writer)
        {
            writer.WriteBit(Active);

            if (!Active) return;

            writer.Write(CannonVelocity);
            writer.Write(CannonReFireRate);
            writer.Write(CannonMinDistance);

            writer.Write(CannonBarrelOffset);

            writer.Write(CannonAngle);
            writer.Write(Facing);

            writer.Write((ulong) Operator.ObjectId);

            writer.Write(CannonTimeout);
            writer.Write(CannonFoV);
        }
    }
}