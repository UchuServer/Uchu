namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct ToggleFreeCamModeMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ToggleFreeCamMode;
    }
}