namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ServerTerminateInteractionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ServerTerminateInteraction;
		public GameObject Terminator { get; set; }
		public TerminateType Type { get; set; }
	}
}