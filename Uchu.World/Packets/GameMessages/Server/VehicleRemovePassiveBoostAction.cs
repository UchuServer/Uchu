namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct VehicleRemovePassiveBoostAction
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VehicleRemovePassiveBoostAction;
	}
}