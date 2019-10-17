using RakDotNet.IO;

namespace Uchu.World
{
    public class OpenVendorWindowMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.VendorOpenWindow;
        
        public override void SerializeMessage(BitWriter writer)
        {
        }
    }
}