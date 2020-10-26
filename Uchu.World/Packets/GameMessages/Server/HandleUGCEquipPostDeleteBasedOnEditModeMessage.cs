using System.Numerics;
using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class HandleUGCEquipPostDeleteBasedOnEditMode : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.HandleUGCEquipPostDeleteBasedOnEditMode;

        public long invItem;
        public int itemsTotal = 0;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(invItem);

            writer.WriteBit(itemsTotal != 0);
            if (itemsTotal != 0) writer.Write(itemsTotal);
        }
    }
}
