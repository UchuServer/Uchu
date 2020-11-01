using RakDotNet.IO;
using Uchu.World.Social;

namespace Uchu.World
{
    public class PropertyEditorBeginMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PropertyEditorBegin;

        public int distanceType = 0;
        public long propertyObjectID = 0;
        public int startMode = 1;
        public bool startPaused = false;

        public override void Deserialize(BitReader reader)
        {
            if (reader.ReadBit()) distanceType = reader.Read<int>();
            if (reader.ReadBit()) propertyObjectID = reader.Read<long>();
            if (reader.ReadBit()) startMode = reader.Read<int>();
            if (reader.ReadBit()) startPaused = reader.ReadBit();
        }
    }
}