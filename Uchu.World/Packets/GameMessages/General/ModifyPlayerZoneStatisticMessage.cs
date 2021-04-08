using System;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class ModifyPlayerZoneStatisticMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.ModifyPlayerZoneStatistic;
        public bool Set { get; set; }
        public string StatName { get; set; }
        public int StatValue { get; set; }
        public ushort ZoneId { get; set; }
        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(Set);
            writer.WriteString(StatName, length: StatName.Length);
            writer.Write(StatValue);
            writer.Write(ZoneId);
        }
    }
}