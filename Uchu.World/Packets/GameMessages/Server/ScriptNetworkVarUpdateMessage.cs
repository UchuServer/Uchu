using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ScriptNetworkVarUpdate : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ScriptNetworkVarUpdate;

        public string LDFInText;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint)LDFInText.Length);
            writer.WriteString(LDFInText, LDFInText.Length, true);

            writer.Write<short>(0);
        }
    }
}
