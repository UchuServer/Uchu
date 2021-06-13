using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetResurrectRestoreValuesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetResurrectRestoreValues;
		[Default(-1)]
		public int ArmorRestore { get; set; }
		[Default(-1)]
		public int HealthRestore { get; set; }
		[Default(-1)]
		public int ImaginationRestore { get; set; }

		public SetResurrectRestoreValuesMessage(GameObject associate = default, int armorRestore = -1, int healthRestore = -1, int imaginationRestore = -1)
		{
			this.Associate = associate;
			this.ArmorRestore = armorRestore;
			this.HealthRestore = healthRestore;
			this.ImaginationRestore = imaginationRestore;
		}
	}
}