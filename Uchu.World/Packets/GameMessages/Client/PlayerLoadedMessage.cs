using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayerLoadedMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayerLoaded;

        public Player Player;
        
        public override void Deserialize(BitReader reader)
        {
            Player = reader.ReadGameObject<Player>(Associate.Zone);
        }
    }
}