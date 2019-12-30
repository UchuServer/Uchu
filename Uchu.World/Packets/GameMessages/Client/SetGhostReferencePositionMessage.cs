using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetGhostReferencePositionMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetGhostReferencePosition;
        
        public Vector3 Position { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Position = reader.Read<Vector3>();
        }
    }
}