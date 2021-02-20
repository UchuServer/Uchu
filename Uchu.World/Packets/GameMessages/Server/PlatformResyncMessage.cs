using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PlatformResyncMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlatformResync;

        public bool Reverse { get; set; }

        public bool StopAtDesiredWaypoint { get; set; }
        
        public int Command { get; set; }
        
        public PlatformState State { get; set; }
        
        public int UnexpectedCommand { get; set; }
        
        public float IdleTimeElapsed { get; set; }
        
        public float MoveTimeElapsed { get; set; }
        
        public float PercentBetweenPoints { get; set; }

        public int DesiredWaypointIndex { get; set; } = -1;
        
        public int Index { get; set; }
        
        public int NextIndex { get; set; }
        
        public Vector3 UnexpectedLocation { get; set; }
        
        public Quaternion UnexpectedRotation { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Reverse);
            writer.WriteBit(StopAtDesiredWaypoint);
            writer.Write(Command);
            writer.Write((int) State);
            writer.Write(UnexpectedCommand);
            writer.Write(IdleTimeElapsed);
            writer.Write(MoveTimeElapsed);
            writer.Write(PercentBetweenPoints);
            writer.Write(PercentBetweenPoints);
            writer.Write(DesiredWaypointIndex);
            writer.Write(Index);
            writer.Write(NextIndex);
            writer.Write(UnexpectedLocation);
            writer.Write(UnexpectedRotation);
        }
    }
}