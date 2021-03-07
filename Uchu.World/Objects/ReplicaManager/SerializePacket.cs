using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World.Objects.ReplicaManager
{
    public class SerializePacket : ISerializable
    {
        /// <summary>
        /// Id to serialize with.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// Game object to construct.
        /// </summary>
        public GameObject GameObject { get; set; }
        
        /// <summary>
        /// Writes the serialization data.
        /// </summary>
        /// <param name="writer">Bit writer for the data.</param>
        public void Serialize(BitWriter writer)
        {
            writer.Write((byte) MessageIdentifier.ReplicaManagerSerialize);

            writer.Write(Id);

            GameObject.WriteSerialize(writer);
        }
    }
}