using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class FetchModelMetadataResponse : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.FetchModelMetadataResponse;

        public long ugID;
        public long objectID;
        public long requestorID;
        public int context;
        public bool hasUGData;
        public bool HasBPData;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(ugID);
            writer.Write(objectID);
            writer.Write(requestorID);
            writer.Write(context);
            writer.Write(hasUGData);
            writer.Write(HasBPData);
        }
    }
}