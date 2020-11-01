using InfectedRose.Core;
using RakDotNet.IO;
using System.Numerics;

namespace Uchu.World
{
    public class UpdateModelFromClientMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UpdateModelFromClient;

        public long modelID;
        public Vector3 position;
        public Quaternion rotation;

        public override void Deserialize(BitReader reader)
        {
            modelID = reader.Read<long>();
            position = new Vector3(reader.Read<float>(), reader.Read<float>(), reader.Read<float>());

            if (reader.ReadBit())
            {
                rotation = reader.ReadNiQuaternion();
            } 
            else
            {
                rotation = Quaternion.Identity;
            }
        }
    }
}