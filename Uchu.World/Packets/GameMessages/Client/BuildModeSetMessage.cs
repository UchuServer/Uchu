using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BuildModeSetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BuildModeSet;
		public bool Start { get; set; }
		[Default(-1)]
		public int DistanceType { get; set; }
		public bool ModePaused { get; set; }
		[Default(1)]
		public int ModeValue { get; set; }
		public GameObject PlayerId { get; set; }
		[Default]
		public Vector3 StartPos { get; set; }

		public BuildModeSetMessage(GameObject associate = default, bool start = default, int distanceType = -1, bool modePaused = default, int modeValue = 1, GameObject playerId = default, Vector3 startPos = default)
		{
			this.Associate = associate;
			this.Start = start;
			this.DistanceType = distanceType;
			this.ModePaused = modePaused;
			this.ModeValue = modeValue;
			this.PlayerId = playerId;
			this.StartPos = startPos;
		}
	}
}