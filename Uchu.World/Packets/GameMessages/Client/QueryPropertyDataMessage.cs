using RakDotNet.IO;

namespace Uchu.World
{
    public class QueryPropertyData : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.QueryPropertyData;

        public override void Deserialize(BitReader reader)
        {

        }
    }
}