using RakDotNet.IO;

namespace Uchu.World
{
    public class NotifyMissionTaskMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyMissionTask;
        
        public int MissionId { get; set; }
        
        public int TaskIndex { get; set; }
        
        public float[] Updates { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(MissionId);
            writer.Write(1 << (TaskIndex + 1));

            writer.Write((byte) Updates.Length);

            foreach (var update in Updates)
            {
                writer.Write(update);
            }
        }
    }
}