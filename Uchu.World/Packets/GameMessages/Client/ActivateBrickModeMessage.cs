using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ActivateBrickModeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ActivateBrickMode;
		[Default]
		public GameObject BuildObjectId { get; set; }
		[Default(BuildType.OnProperty)]
		public BuildType BuildType { get; set; }
		public bool EnterBuildFromWorld { get; set; }
		public bool EnterFlag { get; set; }

		public ActivateBrickModeMessage(GameObject associate = default, GameObject buildObjectId = default, BuildType buildType = BuildType.OnProperty, bool enterBuildFromWorld = true, bool enterFlag = true)
		{
			this.Associate = associate;
			this.BuildObjectId = buildObjectId;
			this.BuildType = buildType;
			this.EnterBuildFromWorld = enterBuildFromWorld;
			this.EnterFlag = enterFlag;
		}
	}
}