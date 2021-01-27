using System.Collections.Generic;
using System.Linq;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyPetTamingPuzzleSelectedMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.NotifyPetTamingPuzzleSelected;
        
        public List<Brick> Bricks { get; set; }
        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write((uint) Bricks.Count);
            foreach (var item in Bricks)
            {
                writer.Write((uint) item.DesignID);
                writer.Write((uint) item.DesignPart.Material);
            }
        }
    }
}