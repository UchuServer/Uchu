using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class ModularBuildFinishMessage : ClientGameMessage
    {
        public override ushort GameMessageId => 0x0449;

        public int[] Modules { get; set; }

        public override void Deserialize(BitStream stream)
        {
            var count = stream.ReadByte();
            Modules = new int[count];

            for (var i = 0; i < count; i++)
                Modules[i] = stream.ReadInt();
        }
    }
}