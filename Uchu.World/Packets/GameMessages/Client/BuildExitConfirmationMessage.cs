using RakDotNet.IO;

namespace Uchu.World
{
    public class BuildExitConfirmationMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.BuildExitConfirmation;

        public Player Player { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Player = reader.ReadGameObject<Player>(Associate.Zone);
        }
    }
}