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

        [XmlArray("flag")]
        [XmlArrayItem("f")]
        public FlagNode[] Flags { get; set; }
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
        public int ConsumableSlotLot { get; set; }

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
        public string ItemLoTs { get; set; }

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
        public int Lot { get; set; }

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
        public long EyebrowStyle { get; set; }

        [XmlAttribute("ess")]
        public long EyeStyle { get; set; }

        [XmlAttribute("hc")]
        public long HairColor { get; set; }

        [XmlAttribute("hs")]
        public long HairStyle { get; set; }

        [XmlAttribute("l")]
        public long PantsColor { get; set; }

        [XmlAttribute("lh")]
        public long Lh { get; set; }

        [XmlAttribute("ms")]
        public long MouthStyle { get; set; }

        [XmlAttribute("rh")]
        public long Rh { get; set; }

        [XmlAttribute("t")]
        public long ShirtColor { get; set; }
    }

    public class CharacterNode
    {
        [XmlAttribute("acct")]
        public long AccountId { get; set; }

        [XmlAttribute("cc")]
        public long Currency { get; set; }

        [XmlAttribute("ft")]
        public int FreeToPlay { get; set; } = 0;

        [XmlAttribute("gm")]
        public int GmLevel { get; set; }

        [XmlAttribute("ls")]
        public long UniverseScore { get; set; }

        [XmlAttribute("stt")]
        public string PlayerStats { get; set; }

        [XmlAttribute("time")]
        public long PlayTime { get; set; }
    }

    public class LevelNode
    {
        [XmlAttribute("l")]
        public long Level { get; set; }
    }

    public class MissionsNode
    {
        [XmlArray("done")]
        [XmlArrayItem("m")]
        public CompletedMissionNode[] CompletedMissions { get; set; }

        [XmlArray("cur")]
        [XmlArrayItem("m")]
        public MissionNode[] CurrentMissions { get; set; }
    }

    public class MissionNode
    {
        [XmlAttribute("id")]
        public long MissionId { get; set; }

        [XmlAttribute("o")]
        public int Unknown { get; set; }

        public bool ShouldSerializeUnknown() => Unknown > 0;

        [XmlElement("sv")]
        public MissionProgressNode[] Progress { get; set; }
    }

    public class MissionProgressNode
    {
        [XmlAttribute("v")]
        public int Value { get; set; }

        public bool ShouldSerializeValue() => Value > 0;
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

    public class FlagNode
    {
        [XmlAttribute("id")]
        public int FlagId { get; set; }

        [XmlAttribute("v")]
        public int Flag { get; set; }
    }
}