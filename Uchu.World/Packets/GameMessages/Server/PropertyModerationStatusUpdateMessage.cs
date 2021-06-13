using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PropertyModerationStatusUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyModerationStatusUpdate;
		[Default(-1)]
		public int NewModerationStatus { get; set; }
		[Wide]
		public string RejectionReason { get; set; }

		public PropertyModerationStatusUpdateMessage(GameObject associate = default, int newModerationStatus = -1, string rejectionReason = default)
		{
			this.Associate = associate;
			this.NewModerationStatus = newModerationStatus;
			this.RejectionReason = rejectionReason;
		}
	}
}