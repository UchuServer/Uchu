namespace Uchu.World
{
    public struct EquipLocation
    {
        public readonly string Location;

        public static implicit operator string(EquipLocation location)
        {
            return location.Location;
        }

        public static implicit operator EquipLocation(string location)
        {
            return new EquipLocation(location);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case string s:
                    return s == Location;
                case EquipLocation l:
                    return l.Location == Location;
                default:
                    return false;
            }
        }

        public bool Equals(EquipLocation other)
        {
            return Location == other.Location;
        }

        public override int GetHashCode()
        {
            return Location != null ? Location.GetHashCode() : 0;
        }

        private EquipLocation(string location)
        {
            Location = location;
        }

        public static EquipLocation SpecialRight => new EquipLocation("special_r");

        public static EquipLocation Hair => new EquipLocation("hair");

        public static EquipLocation Clavicle => new EquipLocation("clavicle");

        public static EquipLocation Legs => new EquipLocation("legs");

        public static EquipLocation TrinketRight => new EquipLocation("trinket_r");

        public static EquipLocation SpecialLeft => new EquipLocation("special_l");

        public static EquipLocation Extra1 => new EquipLocation("Extra_1");

        public static EquipLocation One => new EquipLocation("0");

        public static EquipLocation Chest => new EquipLocation("chest");

        public static EquipLocation KarlTest => new EquipLocation("Karl Test");

        public static EquipLocation Head => new EquipLocation("head");

        public static EquipLocation SpecialBrick => new EquipLocation("special_brick");

        public static EquipLocation MinifigRoot => new EquipLocation("minifig_root");

        public static EquipLocation Extra2 => new EquipLocation("Extra_2");

        public static EquipLocation Extra4 => new EquipLocation("Extra_4");

        public static EquipLocation Extra3 => new EquipLocation("Extra_3");

        public static EquipLocation GreebleRight => new EquipLocation("greeble_r");

        public static EquipLocation GreebleLeft => new EquipLocation("greeble_l");

        public static EquipLocation AccumulationRoot => new EquipLocation("Accumulation_Root");

        public static EquipLocation Extra5 => new EquipLocation("Extra_5");

        public static EquipLocation Root => new EquipLocation("root");

        public static EquipLocation Four => new EquipLocation("4");

        public static EquipLocation Locator1 => new EquipLocation("locator1");

        public static EquipLocation LeftElbow => new EquipLocation("l_elbow");

        public static EquipLocation RightElbow => new EquipLocation("r_elbow");

        public static EquipLocation LeftShoulder => new EquipLocation("l_shoulder");

        public static EquipLocation RightShoulder => new EquipLocation("r_shoulder");

        public static EquipLocation Spine => new EquipLocation("spine");

        public static EquipLocation TaiLeftA => new EquipLocation("tail_a");

        public static EquipLocation TaiLeftB => new EquipLocation("tail_b");

        public static EquipLocation TaiLeftC => new EquipLocation("tail_c");

        public static EquipLocation LeftWrist => new EquipLocation("l_wrist");

        public static EquipLocation RightWrist => new EquipLocation("r_wrist");

        public static EquipLocation Extra6 => new EquipLocation("Extra_6");
    }
}