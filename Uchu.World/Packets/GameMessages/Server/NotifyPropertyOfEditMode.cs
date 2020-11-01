using RakDotNet.IO;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyPropertyOfEditMode : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyPropertyOfEditMode;

        public bool EditingActive;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(EditingActive);
        }
    }
}