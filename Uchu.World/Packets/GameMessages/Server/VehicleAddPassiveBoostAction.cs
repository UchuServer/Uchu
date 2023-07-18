namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct VehicleAddPassiveBoostAction
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VehicleAddPassiveBoostAction;
	}
}