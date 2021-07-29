namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct ActivityStartMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ActivityStart;
    }
}