using System.Xml.Serialization;

namespace Uchu.World.Client
{
    [XmlRoot("triggers")]
    public class Triggers
    {
        [XmlElement("trigger")] public Trigger[] TriggerElements { get; set; }
    }

    public class Trigger
    {
        public int PrimaryId { get; set; }
        
        [XmlAttribute("id")] public int Id { get; set; }
        
        [XmlAttribute("enabled")] public int Enabled { get; set; }
        
        [XmlElement("event")] public TriggerEvent[] Events { get; set; }
    }

    public class TriggerEvent
    {
        [XmlAttribute("id")] public string Id { get; set; }
        
        [XmlElement("command")] public TriggerCommand[] Commands { get; set; }
    }

    public class TriggerCommand
    {
        [XmlAttribute("id")] public string Id { get; set; }
        
        [XmlAttribute("target")] public string Target { get; set; }
        
        [XmlAttribute("targetName")] public string TargetName { get; set; }

        [XmlAttribute("args")] public string Arguments { get; set; }
    }
}