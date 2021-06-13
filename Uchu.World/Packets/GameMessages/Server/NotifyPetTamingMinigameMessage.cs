using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyPetTamingMinigameMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyPetTamingMinigame;
		public GameObject PetId { get; set; }
		public GameObject PlayerTamingId { get; set; }
		public bool ForceTeleport { get; set; }
		public PetTamingNotifyType NotifyType { get; set; }
		public Vector3 PetDestPos { get; set; }
		public Vector3 TelePos { get; set; }
		[Default]
		public Quaternion TeleRot { get; set; }
	}
}