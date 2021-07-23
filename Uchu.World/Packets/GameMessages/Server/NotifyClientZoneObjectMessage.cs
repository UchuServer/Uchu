using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class NotifyClientZoneObjectMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.NotifyClientZoneObject;
        
        public string Name { get; set; }

        public int Param1 { get; set; }

        public int Param2 { get; set; }

        public GameObject ParamObj { get; set; }

        public string ParamStr { get; set; } = "";
        
        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteString(Name, Name.Length, true);
            writer.Write(Param1);
            writer.Write(Param2);
            writer.Write(ParamObj);
            writer.WriteString(ParamStr, ParamStr.Length);
        }
    }
}