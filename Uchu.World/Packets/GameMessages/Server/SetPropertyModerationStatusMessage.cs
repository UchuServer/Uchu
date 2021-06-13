using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetPropertyModerationStatusMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetPropertyModerationStatus;
		[Default(-1)]
		public int ModerationStatus { get; set; }

		public SetPropertyModerationStatusMessage(GameObject associate = default, int moderationStatus = -1)
		{
			this.Associate = associate;
			this.ModerationStatus = moderationStatus;
		}
	}
}