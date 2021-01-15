using System.Collections.Generic;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PetTamingTryBuildMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.PetTamingTryBuild;

        public List<Brick> Bricks = new List<Brick>();
        public bool Failed;
        public override void Deserialize(BitReader reader)
        {
            for (int i = 0; i < reader.Read<uint>(); ++i)
            {
                Brick current = new Brick();
                current.DesignID = reader.Read<uint>();
                current.DesignPart = new Part();
                current.DesignPart.Material = (int)reader.Read<uint>();
            }

            Failed = reader.ReadBit();
        }
    }
}