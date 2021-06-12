using System.Numerics;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct TeleportMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.Teleport;
        public bool NoGravity { get; set; }
        public bool IgnoreY { get; set; }
        public bool SetRotation { get; set; }
        public bool SkipAllChecks { get; set; }
        public Vector3 Position { get; set; }
        public bool UseNavMesh { get; set; }
        public Quaternion Rotation { get; set; }
    }
}