using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct FireClientEventMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.FireEventClientSide;
        [Wide]
        public string Arguments { get; set; }
        public GameObject Target { get; set; }
        [Default]
        public long FirstParameter { get; set; }
        [Default(-1)]
        public int SecondParameter { get; set; }
        public GameObject Sender { get; set; }
    }
}