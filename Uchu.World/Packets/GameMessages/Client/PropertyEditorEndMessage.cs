using RakDotNet.IO;

namespace Uchu.World
{
    public class PropertyEditorEnd : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PropertyEditorEnd;

        public override void Deserialize(BitReader reader) { }
    }
}