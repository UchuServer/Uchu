using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UIMessageServerToSingleClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UIMessageServerToSingleClient;
		// Storing the length as null is intended to specify the length should not be stored.
		[StoreLengthAs(null)]
		public byte[] Content { get; set; }
		public string MessageName { get; set; }
	}
}