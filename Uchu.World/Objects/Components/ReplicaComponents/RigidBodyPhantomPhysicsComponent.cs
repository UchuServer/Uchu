using Uchu.World.Client;

namespace Uchu.World
{
    public class RigidBodyPhantomPhysicsComponent : StructReplicaComponent<RigidBodyPhantomPhysicsSerialization>
    {
        public override ComponentId Id => ComponentId.RigidBodyPhantomPhysicsComponent;
        protected RigidBodyPhantomPhysicsComponent()
        {
            Listen(OnStart, () =>
            {
                // Find physics asset path from cdclient
                var phantomPhysicsComponentId = GameObject.Lot.GetComponentId(ComponentId.RigidBodyPhantomPhysicsComponent);
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
        public override RigidBodyPhantomPhysicsSerialization GetPacket()
        {
            var packet = base.GetPacket();
            packet.HasPosition = true;
            packet.Position = Transform.Position;
            packet.Rotation = Transform.Rotation;
            return packet;
        }
    }
}