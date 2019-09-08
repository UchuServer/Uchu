using RakDotNet.IO;

namespace Uchu.World
{
    public class ReadyForUpdateMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ReadyForUpdates;

        public GameObject GameObject { get; set; }

        public override void Deserialize(BitReader reader)
        {
            GameObject = reader.ReadGameObject(Associate.Zone);
        }
    }
}