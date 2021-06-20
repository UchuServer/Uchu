using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct FireEventServerSideMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FireEventServerSide;
		[Wide]
		public string Arguments { get; set; }
		[Default(-1)]
		public int Param1 { get; set; }
		[Default(-1)]
		public int Param2 { get; set; }
		[Default(-1)]
		public int Param3 { get; set; }
		public GameObject SenderId { get; set; }

		public FireEventServerSideMessage(GameObject associate = default, string arguments = default, int param1 = -1, int param2 = -1, int param3 = -1, GameObject senderId = default)
		{
			this.Associate = associate;
			this.Arguments = arguments;
			this.Param1 = param1;
			this.Param2 = param2;
			this.Param3 = param3;
			this.SenderId = senderId;
		}
	}
}