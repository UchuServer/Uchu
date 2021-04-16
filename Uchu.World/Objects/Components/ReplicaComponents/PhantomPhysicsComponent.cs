using System.Linq;
using System.Numerics;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Physics;
using Uchu.World.Client;

namespace Uchu.World
{
    public class PhantomPhysicsComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.PhantomPhysicsComponent;

        public bool HasPosition { get; set; } = true;

        public bool IsEffectActive { get; set; }

        public PhantomPhysicsEffectType EffectType { get; set; }

        public float EffectAmount { get; set; }
        
        public bool AffectedByDistance { get; set; }

        public float MinDistance { get; set; }
        
        public float MaxDistance { get; set; }
        
        public Vector3 EffectDirection { get; set; }

        protected PhantomPhysicsComponent()
        {
            Listen(OnStart, () =>
            {
                if (GameObject.Settings.ContainsKey("POI"))
                {
                    var physicsComponent = GameObject.AddComponent<PhysicsComponent>();
                    var phantomPhysicsComponentId = GameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                    var cdcComponent = ClientCache.GetTable<Core.Client.PhysicsComponent>()
                        .FirstOrDefault(r => r.Id == phantomPhysicsComponentId);
                    var assetPath = cdcComponent?.Physicsasset;
                    Logger.Warning($"## Set {phantomPhysicsComponentId} path to {assetPath}");
                    physicsComponent.SetPhysicsByPath(assetPath);
                }
            });
        }

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (writer.Flag(HasPosition))
            {
                writer.Write(Transform.Position);
                writer.Write(Transform.Rotation);
            }

            writer.WriteBit(true);
            
            if (!writer.Flag(IsEffectActive)) return;

            writer.Write((uint) EffectType);
            writer.Write(EffectAmount);

            if (writer.Flag(AffectedByDistance))
            {
                writer.Write(MinDistance);
                writer.Write(MaxDistance);
            }

            if (!writer.Flag(EffectDirection != Vector3.Zero)) return;

            writer.Write(EffectDirection * EffectAmount);
        }
    }
}