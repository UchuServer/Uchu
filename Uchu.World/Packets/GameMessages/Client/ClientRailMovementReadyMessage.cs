using RakDotNet.IO;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct ClientRailMovementReadyMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ClientRailMovementReady;
    }
}