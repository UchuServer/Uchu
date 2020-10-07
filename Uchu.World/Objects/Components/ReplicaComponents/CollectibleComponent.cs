using RakDotNet.IO;

namespace Uchu.World
{
    [RequireComponent(typeof(DestroyableComponent))]
    public class CollectibleComponent : ReplicaComponent
    {
        public ushort CollectibleId { get; private set; }

        public override ComponentId Id => ComponentId.CollectibleComponent;

        protected CollectibleComponent()
        {
            Listen(OnStart, () =>
            {
                CollectibleId = (ushort) (int) GameObject.Settings["collectible_id"];

                foreach (var stats in GameObject.GetComponents<DestroyableComponent>()) stats.HasStats = false;
            });
        }

        public override void Construct(BitWriter writer)
        {
            if (!GameObject.TryGetComponent<DestructibleComponent>(out _))
                GameObject.GetComponent<DestroyableComponent>().Construct(writer);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (!GameObject.TryGetComponent<DestructibleComponent>(out _))
                GameObject.GetComponent<DestroyableComponent>().Serialize(writer);

            writer.Write(CollectibleId);
        }
    }
}