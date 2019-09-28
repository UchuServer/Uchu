using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class RequestClientBounceMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestClientBounce;

        public GameObject BounceTarget { get; set; }
        
        public Vector3 TargetPosition { get; set; }
        
        public Vector3 BouncedObjectPosition { get; set; }
        
        public GameObject RequestSource { get; set; }
        
        public bool AllBounced { get; set; }
        
        public bool AllowClientOverride { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(BounceTarget);

            writer.Write(TargetPosition);
            writer.Write(BouncedObjectPosition);

            writer.Write(RequestSource);

            writer.WriteBit(AllBounced);
            writer.WriteBit(AllowClientOverride);
        }
    }
}