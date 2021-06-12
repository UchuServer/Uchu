namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct MatchResponse
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.MatchResponse;
        public int Response { get; set; } // TODO: Can/should this be an enum?
    }
}