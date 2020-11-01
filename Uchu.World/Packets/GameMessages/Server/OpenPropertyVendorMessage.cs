using RakDotNet.IO;

namespace Uchu.World
{
    public class OpenPropertyVendorMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.OpenPropertyVendor;

        public override void SerializeMessage(BitWriter writer)
        {

        }
    }
}