using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetRailMovementMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetRailMovement;
		public bool PathGoForward { get; set; }
		[Wide]
		public string PathName { get; set; }
		public uint PathStart { get; set; }
		[Default(-1)]
		public int RailActivatorComponentId { get; set; }
		[Default]
		public GameObject RailActivatorObjId { get; set; }

		public SetRailMovementMessage(GameObject associate = default, bool pathGoForward = default, string pathName = default, uint pathStart = default, int railActivatorComponentId = -1, GameObject railActivatorObjId = default)
		{
			this.Associate = associate;
			this.PathGoForward = pathGoForward;
			this.PathName = pathName;
			this.PathStart = pathStart;
			this.RailActivatorComponentId = railActivatorComponentId;
			this.RailActivatorObjId = railActivatorObjId;
		}
	}
}