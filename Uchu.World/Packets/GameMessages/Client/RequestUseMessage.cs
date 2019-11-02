using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RequestUseMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequestUse;

        public bool IsMultiInteract { get; set; }

        public uint MultiInteractId { get; set; }

        public int MultiInteractType { get; set; }

        public GameObject TargetObject { get; set; }

        public bool Secondary { get; set; }

        public override void Deserialize(BitReader reader)
        {
            IsMultiInteract = reader.ReadBit();

            MultiInteractId = reader.Read<uint>();

            MultiInteractType = reader.Read<int>();

            TargetObject = reader.ReadGameObject(Associate.Zone);

            Secondary = reader.ReadBit();
        }
    }
}