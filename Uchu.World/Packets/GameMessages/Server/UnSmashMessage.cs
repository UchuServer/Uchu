using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UnSmashMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UnSmash;
		[Default]
		public GameObject BuilderId { get; set; }
		[Default(3)]
		public float Duration { get; set; }

		public UnSmashMessage(GameObject associate = default, GameObject builderId = default, float duration = 3)
		{
			this.Associate = associate;
			this.BuilderId = builderId;
			this.Duration = duration;
		}
	}
}