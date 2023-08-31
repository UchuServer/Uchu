namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct PopEquippedItemsStateMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PopEquippedItemsState;
    }
}