using System.Xml.Serialization;

namespace Uchu.World
{
    [XmlType("obj")]
    public class XmlData
    {
        [XmlAttribute("v")]
        public int Version { get; set; } = 1;

        [XmlElement("buff")]
        public object Buff { get; set; }

        [XmlElement("dest")]
        public DestNode Stats { get; set; }

        [XmlElement("inv")]
        public InventoryNode Inventory { get; set; }

        [XmlElement("mf")]
        public MinifigureNode Minifigure { get; set; }

        [XmlElement("char")]
        public CharacterNode Character { get; set; }

        [XmlElement("lvl")]
        public LevelNode Level { get; set; }

        [XmlElement("mis")]
        public MissionsNode Missions { get; set; }
    }

    [XmlType("dest")]
    public class DestNode
    {
        [XmlAttribute("am")]
        public int MaximumArmor { get; set; }

        [XmlAttribute("ac")]
        public int CurrentArmor { get; set; }

        [XmlAttribute("hc")]
        public int CurrentHealth { get; set; }

        [XmlAttribute("hm")]
        public int MaximumHealth { get; set; }

        [XmlAttribute("ic")]
        public int CurrentImagination { get; set; }

        [XmlAttribute("im")]
        public int MaximumImagination { get; set; }
    }

    [XmlType("inv")]
    public class InventoryNode
    {
        [XmlAttribute("csl")]
        public int ConsumableSlotLOT { get; set; }

        [XmlArray("bag")]
        public BagNode[] Bags { get; set; }

        [XmlArray("grps")]
        public GroupNode[] Groups { get; set; }

        [XmlArray("items")]
        [XmlArrayItem("in")]
        public ItemContainerNode[] ItemContainers { get; set; }
    }

    [XmlType("b")]
    public class BagNode
    {
        [XmlAttribute("m")]
        public int Slots { get; set; }

        [XmlAttribute("t")]
        public int Type { get; set; }
    }

    [XmlType("grp")]
    public class GroupNode
    {
        [XmlAttribute("id")]
        public string GroupId { get; set; }

        [XmlAttribute("l")]
        public string ItemLOTs { get; set; }

        [XmlAttribute("n")]
        public string Name { get; set; }

        [XmlAttribute("t")]
        public int Type { get; set; }
    }

    public class ItemContainerNode
    {
        [XmlAttribute("t")]
        public int Type { get; set; }

        [XmlElement("i")]
        public ItemNode[] Items { get; set; }
    }

    public class ItemNode
    {
        [XmlAttribute("b")]
        public int Bound { get; set; }

        [XmlAttribute("c")]
        public int Count { get; set; }

        [XmlAttribute("eq")]
        public int Equipped { get; set; }

        [XmlAttribute("id")]
        public long ObjectId { get; set; }

        [XmlAttribute("l")]
        public int LOT { get; set; }

        [XmlAttribute("s")]
        public int Slot { get; set; }

        [XmlElement("x")]
        public ExtraInfoNode ExtraInfo { get; set; }
    }

    public class ExtraInfoNode
    {
        [XmlAttribute("ma")]
        public string ModuleAssemblyInfo { get; set; }
    }

    public class MinifigureNode
    {
        [XmlAttribute("es")]
        public int EyebrowStyle { get; set; }

        [XmlAttribute("ess")]
        public int EyeStyle { get; set; }

        [XmlAttribute("hc")]
        public int HairColor { get; set; }

        [XmlAttribute("hs")]
        public int HairStyle { get; set; }

        [XmlAttribute("lh")]
        public int Lh { get; set; }

        [XmlAttribute("rh")]
        public int Rh { get; set; }

        [XmlAttribute("t")]
        public int ShirtColor { get; set; }
    }

    public class CharacterNode
    {
        [XmlAttribute("acct")]
        public long AccountId { get; set; }

        [XmlAttribute("cc")]
        public int Currency { get; set; }

        [XmlAttribute("ft")]
        public int FreeToPlay { get; set; } = 0;

        [XmlAttribute("gm")]
        public int GMLevel { get; set; }

        [XmlAttribute("ls")]
        public int UniverseScore { get; set; }

        [XmlAttribute("stt")]
        public string PlayerStats { get; set; }

        [XmlAttribute("time")]
        public long PlayTime { get; set; }
    }

    public class LevelNode
    {
        [XmlAttribute("l")]
        public int Level { get; set; }
    }

    public class MissionsNode
    {
        [XmlArray("cur")]
        [XmlArrayItem("m")]
        public MissionNode[] CurrentMissions { get; set; }

        [XmlArray("done")]
        [XmlArrayItem("m")]
        public CompletedMissionNode[] CompletedMissions { get; set; }
    }

    public class MissionNode
    {
        [XmlAttribute("id")]
        public long MissionId { get; set; }

        [XmlElement("sv")]
        public MissionProgressNode Progress { get; set; }
    }

    public class MissionProgressNode
    {
        [XmlAttribute("v")]
        public long Value { get; set; }
    }

    public class CompletedMissionNode
    {
        [XmlAttribute("cct")]
        public int CompletionCount { get; set; }

        [XmlAttribute("cts")]
        public long LastCompletion { get; set; }

        [XmlAttribute("id")]
        public long MissionId { get; set; }
    }
}