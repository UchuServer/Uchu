using System;
using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MatchUpdate : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MatchUpdate;
        
        public string Data { get; set; }
        
        public int Type { get; set; } // TODO: Convert to enum
        
        public override void SerializeMessage(BitWriter writer)
        {
            var ldf = Data.ToString();
            writer.Write((uint) ldf.Length);

            if (ldf.Length > 0)
            {
                writer.WriteString(ldf, ldf.Length, true);
            }

            writer.Write(Type);
        }
    }
}