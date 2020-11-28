using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ScriptNetworkVarUpdateMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ScriptNetworkVarUpdate;

        public string LDFInText;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint)LDFInText.Length);
            writer.WriteString(LDFInText, LDFInText.Length, true);

            if (LDFInText.Length != 0)
            {
                writer.Write<short>(0);
            }
        }
    }
}