using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayerSetCameraCyclingModeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.PlayerSetCameraCyclingMode;

        public bool AllowCyclingWhileDeadOnly { get; set; }

        public CyclingMode CyclingMode { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(AllowCyclingWhileDeadOnly);

            if (writer.Flag(CyclingMode != CyclingMode.AllowCycleTeammates))
                writer.Write((uint) CyclingMode);
        }
    }
}