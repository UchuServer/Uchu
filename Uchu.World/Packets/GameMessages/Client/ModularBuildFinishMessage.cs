using RakDotNet.IO;

namespace Uchu.World
{
    public class ModularBuildFinishMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ModularBuildFinish;
        
        public Lot[] Modules { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Modules = new Lot[reader.Read<byte>()];

            for (var i = 0; i < Modules.Length; i++)
            {
                Modules[i] = reader.Read<Lot>();
            }
        }
    }
}