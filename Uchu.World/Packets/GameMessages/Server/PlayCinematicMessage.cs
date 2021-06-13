using Uchu.World.Systems.Behaviors;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayCinematicMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayCinematic;
		public bool AllowGhostUpdates { get; set; }
		public bool CloseMultiInteract { get; set; }
		public bool SendServerNotify { get; set; }
		public bool UseControlledObjectForAudioListener { get; set; }
		[Default]
		public EndBehavior EndBehavior { get; set; }
		public bool HidePlayerDuringCine { get; set; }
		[Default(-1)]
		public float LeadIn { get; set; }
		public bool LeavePlayerLockedWhenFinished { get; set; }
		public bool LockPlayer { get; set; }
		[Wide]
		public string PathName { get; set; }
		public bool Result { get; set; }
		public bool SkipIfSamePath { get; set; }
		public float StartTimeAdvance { get; set; }

		public PlayCinematicMessage(GameObject associate = default, bool allowGhostUpdates = true, bool closeMultiInteract = default, bool sendServerNotify = default, bool useControlledObjectForAudioListener = default, EndBehavior endBehavior = default, bool hidePlayerDuringCine = default, float leadIn = -1, bool leavePlayerLockedWhenFinished = default, bool lockPlayer = true, string pathName = default, bool result = default, bool skipIfSamePath = default, float startTimeAdvance = default)
		{
			this.Associate = associate;
			this.AllowGhostUpdates = allowGhostUpdates;
			this.CloseMultiInteract = closeMultiInteract;
			this.SendServerNotify = sendServerNotify;
			this.UseControlledObjectForAudioListener = useControlledObjectForAudioListener;
			this.EndBehavior = endBehavior;
			this.HidePlayerDuringCine = hidePlayerDuringCine;
			this.LeadIn = leadIn;
			this.LeavePlayerLockedWhenFinished = leavePlayerLockedWhenFinished;
			this.LockPlayer = lockPlayer;
			this.PathName = pathName;
			this.Result = result;
			this.SkipIfSamePath = skipIfSamePath;
			this.StartTimeAdvance = startTimeAdvance;
		}
	}
}