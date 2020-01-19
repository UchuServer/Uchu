using RakDotNet.IO;

namespace Uchu.World
{
    [RequireComponent(typeof(Stats))]
    public class CollectibleComponent : ReplicaComponent
    {
        public ushort CollectibleId { get; private set; }

        public override ComponentId Id => ComponentId.CollectibleComponent;

        protected CollectibleComponent()
        {
            Listen(OnStart, () =>
            {
                CollectibleId = (ushort) (int) GameObject.Settings["collectible_id"];

                foreach (var stats in GameObject.GetComponents<Stats>()) stats.HasStats = false;
            });
        }

        public override void Construct(BitWriter writer)
        {
            if (!GameObject.TryGetComponent<DestructibleComponent>(out _))
                GameObject.GetComponent<Stats>().Construct(writer);

            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            if (!GameObject.TryGetComponent<DestructibleComponent>(out _))
                GameObject.GetComponent<Stats>().Serialize(writer);

            writer.Write(CollectibleId);
        }
    }
}