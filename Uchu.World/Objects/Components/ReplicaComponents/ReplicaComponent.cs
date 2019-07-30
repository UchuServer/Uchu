using RakDotNet.IO;

namespace Uchu.World
{
    [Essential]
    public abstract class ReplicaComponent : Component
    {
        /// <summary>
        /// Id of this ReplicaComponent.
        /// </summary>
        public abstract ReplicaComponentsId Id { get; }

        /// <summary>
        /// The data that is only sent once to each client.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Construct(BitWriter writer);

        /// <summary>
        /// The data that is sent every time an update accrues.
        /// </summary>
        /// <param name="writer"></param>
        public abstract void Serialize(BitWriter writer);
    }
}