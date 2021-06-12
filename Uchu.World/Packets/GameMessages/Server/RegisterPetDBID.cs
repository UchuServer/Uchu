namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RegisterPetDBIDMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RegisterPetDBID;
        public GameObject PetItemObject { get; set; }
    }
}