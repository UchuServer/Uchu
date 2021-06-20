using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetMissionTypeStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetMissionTypeState;
		[Default(MissionLockState.New)]
		public MissionLockState LockState { get; set; }
		public string SubType { get; set; }
		public string Type { get; set; }

		public SetMissionTypeStateMessage(GameObject associate = default, MissionLockState lockState = MissionLockState.New, string subType = default, string type = default)
		{
			this.Associate = associate;
			this.LockState = lockState;
			this.SubType = subType;
			this.Type = type;
		}
	}
}