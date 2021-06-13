using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetJetPackModeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetJetPackMode;
		public bool BypassChecks { get; set; }
		public bool DoHover { get; set; }
		public bool Use { get; set; }
		[Default(-1)]
		public int EffectId { get; set; }
		[Default(10)]
		public float AirSpeed { get; set; }
		[Default(15)]
		public float MaxAirSpeed { get; set; }
		[Default]
		public float VerticalVelocity { get; set; }
		[Default(-1)]
		public int WarningEffectId { get; set; }

		public SetJetPackModeMessage(GameObject associate = default, bool bypassChecks = default, bool doHover = default, bool use = default, int effectId = -1, float airSpeed = 10, float maxAirSpeed = 15, float verticalVelocity = 1, int warningEffectId = -1)
		{
			this.Associate = associate;
			this.BypassChecks = bypassChecks;
			this.DoHover = doHover;
			this.Use = use;
			this.EffectId = effectId;
			this.AirSpeed = airSpeed;
			this.MaxAirSpeed = maxAirSpeed;
			this.VerticalVelocity = verticalVelocity;
			this.WarningEffectId = warningEffectId;
		}
	}
}