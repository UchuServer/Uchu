using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class HandleUGCEquipPreCreateBasedOnEditModeMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.HandleUGCEquipPreCreateBasedOnEditMode;

        bool OnCursor;
        int modelCount;
        long modelID;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(OnCursor);
            writer.Write(modelCount);
            writer.Write(modelID);
        }
    }
}