using RakDotNet.IO;
using Uchu.World.Social;

namespace Uchu.World
{
    public class FetchModelMetadataRequest : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.BuybackFromVendor;

        public int context;
        public long objectID;
        public long requestorID;
        public long ugID;

        public override void Deserialize(BitReader reader)
        {
            context = reader.Read<int>();
            objectID = reader.Read<long>();
            requestorID = reader.Read<long>();
            ugID = reader.Read<long>();
        }
    }
}