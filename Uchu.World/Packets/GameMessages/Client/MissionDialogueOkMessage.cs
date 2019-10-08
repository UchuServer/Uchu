using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MissionDialogueOkMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MissionDialogueOK;

        public bool IsComplete { get; set; }

        public MissionState MissionState { get; set; }

        public int MissionId { get; set; }

        public GameObject Responder { get; set; }

        public override void Deserialize(BitReader reader)
        {
            IsComplete = reader.ReadBit();

            MissionState = (MissionState) reader.Read<int>();

            MissionId = reader.Read<int>();

            Responder = reader.ReadGameObject(Associate.Zone);
        }
    }
}
