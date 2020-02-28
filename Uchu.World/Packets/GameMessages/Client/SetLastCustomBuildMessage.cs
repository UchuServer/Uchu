using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class SetLastCustomBuildMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.SetLastCustomBuild;

        public string Tokens { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Tokens = reader.ReadString((int) reader.Read<uint>(), true);
        }
    }
}