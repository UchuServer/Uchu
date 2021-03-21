using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World.Objects.ReplicaManager
{
    public class ConstructionPacket : ISerializable
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
            writer.Write((byte) MessageIdentifier.ReplicaManagerConstruction);

            writer.WriteBit(true);
            writer.Write(Id);

            GameObject.WriteConstruct(writer);
        }
    }
}