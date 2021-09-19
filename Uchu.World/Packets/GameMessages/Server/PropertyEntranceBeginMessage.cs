namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct PropertyEntranceBeginMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PropertyEntranceBegin;
    }
}
