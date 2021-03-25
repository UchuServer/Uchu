using RakDotNet;
using RakDotNet.IO;

namespace Uchu.World.Objects.ReplicaManager
{
    public class DestructionPacket: ISerializable
    {
        /// <summary>
        /// Id to serialize with.
        /// </summary>
        public ushort Id { get; set; }
        
        /// <summary>
        /// Writes the serialization data.
        /// </summary>
        /// <param name="writer">Bit writer for the data.</param>
        public void Serialize(BitWriter writer)
        {
            writer.Write((byte) MessageIdentifier.ReplicaManagerDestruction);

            writer.Write(Id);
        }
    }
}