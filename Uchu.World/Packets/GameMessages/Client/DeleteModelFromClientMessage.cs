using RakDotNet.IO;
using Uchu.World.Social;

namespace Uchu.World
{
    public class DeleteModelFromClient : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.DeleteModelFromClient;

        public long modelID = 0;
        public DeleteReason reason = DeleteReason.PickingUpModel;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit()) modelID = reader.Read<long>();
            if (reader.ReadBit()) reason = (DeleteReason)reader.Read<int>();
        }
    }
}