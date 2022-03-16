using System.Linq;
using System.Numerics;
using RakDotNet.IO;
using Uchu.World.Client;

namespace Uchu.World
{
    public class PhantomPhysicsComponent : StructReplicaComponent<PhantomPhysicsSerialization>
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
                // In this case, physics will be set by the PrimitiveModelComponent
                if (GameObject.Settings.ContainsKey("primitiveModelType"))
                    return;

                // This is for POIs with a POI key in the settings, used for example everywhere in AG except for
                // Monument Orange Path (AG_Mon_3).
                // Not all POIs are handled this way, for example those in GF are done using triggers.
                // Also handles respawn volumes (when the player reaches these, it'll be their new respawn location)

                // Find physics asset path from cdclient
                var phantomPhysicsComponentId = GameObject.Lot.GetComponentId(ComponentId.PhantomPhysicsComponent);
                var cdcComponent = ClientCache.Find<Core.Client.PhysicsComponent>(phantomPhysicsComponentId);
                var assetPath = cdcComponent?.Physicsasset;

                // Give physics object correct dimensions
                var physicsComponent = GameObject.AddComponent<PhysicsComponent>();
                physicsComponent.SetPhysicsByPath(assetPath);
            });
        }

        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public override PhantomPhysicsSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.HasPosition = true;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            packet.HasEffectInfo = true;
            packet.IsEffectActive = IsEffectActive;
            packet.EffectType = EffectType;
            packet.EffectAmount = EffectAmount;
            packet.AffectedByDistance = false;
            /*
            packet.MinDistance = MinDistance;
            packet.MaxDistance = MaxDistance;
            */
            packet.IsDirectional = true;
            packet.EffectDirection = EffectDirection;
            //Logger.Log();
            return packet;
        }
    }
}
