using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class SetBuildModeConfirmed : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetBuildModeConfirmed;
        
        public bool Start { get; set; }

        public bool WarnVisitors { get; set; }

        public bool ModePaused { get; set; } = true;

        public int ModeValue { get; set; } = 1;
        
        public Player Player { get; set; }
        
        public Vector3 StartPosition { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Start);

            writer.WriteBit(WarnVisitors);

            writer.WriteBit(ModePaused);

            if (writer.Flag(ModeValue != 1))
            {
                writer.Write(ModeValue);
            }

            writer.Write(Player);

            if (writer.Flag(StartPosition != Vector3.Zero))
            {
                writer.Write(StartPosition);
            }
        }
    }
}