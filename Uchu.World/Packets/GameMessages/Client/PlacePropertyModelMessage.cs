using RakDotNet.IO;

namespace Uchu.World
{
    public class PlacePropertyModel : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlacePropertyModel;

        public long modelID;

        public override void Deserialize(BitReader reader)
        {
            modelID = reader.Read<long>();
        }
    }
}