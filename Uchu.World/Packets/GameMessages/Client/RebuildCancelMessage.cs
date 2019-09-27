using RakDotNet.IO;

namespace Uchu.World
{
    public class RebuildCancelMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RebuildCancel;

        public bool IsReleasedEarly { get; set; }

        public Player Player;
        
        public override void Deserialize(BitReader reader)
        {
            IsReleasedEarly = reader.ReadBit();
            Player = reader.ReadGameObject<Player>(Associate.Zone);
        }
    }
}