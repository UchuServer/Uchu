using RakDotNet.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public class UpdatePropertyModelCountMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.UpdatePropertyModelCount;

        public uint ModelCount;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(ModelCount);
        }
    }
}