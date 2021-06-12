namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct ChangeObjectWorldStateMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ChangeObjectWorldState;
        public ObjectWorldState State { get; set; }
    }
}