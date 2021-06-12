namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RegisterPetIDMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RegisterPetID;
        public GameObject Pet { get; set; }
    }
}