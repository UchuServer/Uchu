using System;
using RakDotNet.IO;

namespace Uchu.World
{
    public class RequeryPropertyModelMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RequeryPropertyModels;

        public override void SerializeMessage(BitWriter writer) { }
    }
}