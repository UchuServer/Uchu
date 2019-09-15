using RakDotNet.IO;

namespace Uchu.World
{
    public class HasBeenCollectedMessage : ClientGameMessage
    {
        public Player Player;
        public override GameMessageId GameMessageId => GameMessageId.HasBeenCollected;

        public override void Deserialize(BitReader reader)
        {
            Player = reader.ReadGameObject<Player>(Associate.Zone);
        }
    }
}