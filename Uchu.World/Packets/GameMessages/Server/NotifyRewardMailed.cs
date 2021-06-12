using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct NotifyRewardMailed
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.NotifyRewardMailed;
        public ObjectId ObjectId { get; set; }
        public Vector3 StartPoint { get; set; }
        public ObjectId Subkey { get; set; }
        public Lot TemplateId { get; set; }
    }
}