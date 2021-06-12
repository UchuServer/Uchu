using System.Numerics;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RequestClientBounceMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RequestClientBounce;
        public GameObject BounceTarget { get; set; }
        public Vector3 TargetPosition { get; set; }
        public Vector3 BouncedObjectPosition { get; set; }
        public GameObject RequestSource { get; set; }
        public bool AllBounced { get; set; }
        public bool AllowClientOverride { get; set; }
    }
}