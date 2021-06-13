namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct AddPetToPlayerMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.AddPetToPlayer;
        public int ElementalType { get; set; }
        public string Name { get; set; }
        public GameObject PetDBId { get; set; }
        public Lot PetLot { get; set; }
    }
}