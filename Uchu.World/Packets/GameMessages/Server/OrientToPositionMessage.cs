using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class OrientToPositionMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.OrientToPosition;
        
        public Vector3 Position { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Position);
        }
    }
}