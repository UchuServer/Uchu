using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayAnimationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayAnimation;
		[Wide]
		public string AnimationId { get; set; }
		public bool ExpectAnimToExist { get; set; }
		public bool PlayImmediate { get; set; }
		public bool TriggerOnCompleteMessage { get; set; }
		[Default(0.4f)]
		public float Priority { get; set; }
		[Default(1)]
		public float Scale { get; set; }

		public PlayAnimationMessage(GameObject associate = default, string animationId = default, bool expectAnimToExist = true, bool playImmediate = default, bool triggerOnCompleteMessage = default, float priority = 0.4f, float scale = 1)
		{
			this.Associate = associate;
			this.AnimationId = animationId;
			this.ExpectAnimToExist = expectAnimToExist;
			this.PlayImmediate = playImmediate;
			this.TriggerOnCompleteMessage = triggerOnCompleteMessage;
			this.Priority = priority;
			this.Scale = scale;
		}
	}
}