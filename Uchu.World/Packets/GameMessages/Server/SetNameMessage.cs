using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class SetNameMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetName;

        public string Name { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint)Name.Length);
            writer.WriteString(Name, Name.Length, true);
        }
    }
}