namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct ResurrectMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.Resurrect;
        public bool ResurrectImminently { get; set; }
    }
}