using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class FireClientEventMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.FireEventClientSide;
        
        public string Arguments { get; set; }
        
        public GameObject Target { get; set; }
        
        public long FirstParameter { get; set; }

        public int SecondParameter { get; set; } = -1;
        
        public GameObject Sender { get; set; }
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) Arguments.Length);
            writer.WriteString(Arguments, Arguments.Length, true);

            writer.Write(Target);

            if (writer.Flag(FirstParameter != default))
            {
                writer.Write(FirstParameter);
            }

            if (writer.Flag(SecondParameter != -1))
            {
                writer.Write(SecondParameter);
            }

            writer.Write(Sender);
        }
    }
}