using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StartRailMovementMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartRailMovement;
		public bool DamageImmune { get; set; }
		public bool NoAggro { get; set; }
		public bool NotifyActivator { get; set; }
		public bool ShowNameBillboard { get; set; }
		public bool CameraLocked { get; set; }
		public bool CollisionEnabled { get; set; }
		[Wide]
		public string LoopSound { get; set; }
		public bool PathGoForward { get; set; }
		[Wide]
		public string PathName { get; set; }
		[Default]
		public uint PathStart { get; set; }
		[Default(-1)]
		public int RailActivatorComponentId { get; set; }
		[Default]
		public GameObject RailActivatorObjId { get; set; }
		[Wide]
		public string StartSound { get; set; }
		[Wide]
		public string StopSound { get; set; }
		public bool UseDb { get; set; }

		public StartRailMovementMessage(GameObject associate = default, bool damageImmune = true, bool noAggro = true, bool notifyActivator = default, bool showNameBillboard = true, bool cameraLocked = true, bool collisionEnabled = true, string loopSound = default, bool pathGoForward = true, string pathName = default, uint pathStart = default, int railActivatorComponentId = -1, GameObject railActivatorObjId = default, string startSound = default, string stopSound = default, bool useDb = true)
		{
			this.Associate = associate;
			this.DamageImmune = damageImmune;
			this.NoAggro = noAggro;
			this.NotifyActivator = notifyActivator;
			this.ShowNameBillboard = showNameBillboard;
			this.CameraLocked = cameraLocked;
			this.CollisionEnabled = collisionEnabled;
			this.LoopSound = loopSound;
			this.PathGoForward = pathGoForward;
			this.PathName = pathName;
			this.PathStart = pathStart;
			this.RailActivatorComponentId = railActivatorComponentId;
			this.RailActivatorObjId = railActivatorObjId;
			this.StartSound = startSound;
			this.StopSound = stopSound;
			this.UseDb = useDb;
		}
	}
}