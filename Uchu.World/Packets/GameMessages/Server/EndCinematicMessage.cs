using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct EndCinematicMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.EndCinematic;
		[Default(-1)]
		public float LeadOut { get; set; }
		public bool LeavePlayerLocked { get; set; }
		[Wide]
		public string PathName { get; set; }

		public EndCinematicMessage(GameObject associate = default, float leadOut = -1, bool leavePlayerLocked = default, string pathName = default)
		{
			this.Associate = associate;
			this.LeadOut = leadOut;
			this.LeavePlayerLocked = leavePlayerLocked;
			this.PathName = pathName;
		}
	}
}