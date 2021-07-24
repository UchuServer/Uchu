namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RegisterPetDbIdMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RegisterPetDBId;
		public GameObject Pet { get; set; }
	}
}