using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyClientFailedPreconditionMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyClientFailedPrecondition;

        public string Reason { get; set; }
        
        public int Id { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) Reason.Length);
            writer.WriteString(Reason, Reason.Length, true);

            writer.Write(Id);
        }
    }
}