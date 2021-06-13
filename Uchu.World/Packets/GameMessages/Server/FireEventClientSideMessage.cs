using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct FireEventClientSideMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FireEventClientSide;
		[Wide]
		public string Arguments { get; set; }
		public GameObject Target { get; set; }
		[Default]
		public long Parameter1 { get; set; }
		[Default(-1)]
		public int Parameter2 { get; set; }
		public GameObject Sender { get; set; }

		public FireEventClientSideMessage(GameObject associate = default, string arguments = default, GameObject target = default, long parameter1 = default, int parameter2 = -1, GameObject sender = default)
		{
			this.Associate = associate;
			this.Arguments = arguments;
			this.Target = target;
			this.Parameter1 = parameter1;
			this.Parameter2 = parameter2;
			this.Sender = sender;
		}
	}
}