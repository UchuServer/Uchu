using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct CinematicUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.CinematicUpdate;
		[Default]
		public CinematicEvent Event { get; set; }
		[Default(-1)]
		public float OverallTime { get; set; }
		[Wide]
		public string PathName { get; set; }
		[Default(-1)]
		public float PathTime { get; set; }
		[Default(-1)]
		public int Waypoint { get; set; }

		public CinematicUpdateMessage(GameObject associate = default, CinematicEvent cinematicEvent = default, float overallTime = -1, string pathName = default, float pathTime = -1, int waypoint = -1)
		{
			this.Associate = associate;
			this.Event = cinematicEvent;
			this.OverallTime = overallTime;
			this.PathName = pathName;
			this.PathTime = pathTime;
			this.Waypoint = waypoint;
		}
	}
}