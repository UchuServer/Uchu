using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetBuildModeConfirmedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetBuildModeConfirmed;
		public bool Start { get; set; }
		public bool WarnVisitors { get; set; }
		public bool ModePaused { get; set; }
		[Default(1)]
		public int ModeValue { get; set; }
		public Player Player { get; set; }
		[Default]
		public Vector3 StartPosition { get; set; }

		public SetBuildModeConfirmedMessage(GameObject associate = default, bool start = default, bool warnVisitors = true, bool modePaused = default, int modeValue = 1, Player player = default, Vector3 startPosition = default)
		{
			this.Associate = associate;
			this.Start = start;
			this.WarnVisitors = warnVisitors;
			this.ModePaused = modePaused;
			this.ModeValue = modeValue;
			this.Player = player;
			this.StartPosition = startPosition;
		}
	}
}