using System.Linq;
using Uchu.Core.CdClient;

namespace Uchu.World
{
    public struct Lot
    {
        public readonly int Id;

        public Lot(int id)
        {
            Id = id;
        }

        public static implicit operator int(Lot lot)
        {
            return lot.Id;
        }

        public static implicit operator Lot(int id)
        {
            return new Lot(id);
        }

        public override bool Equals(object obj)
        {
            return obj switch
            {
                int i => (i == Id),
                Lot l => (l.Id == Id),
                _ => false
            };
        }

        public bool Equals(Lot other)
        {
            return Id == other.Id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public Objects Object
        {
            get
            {
                using var cdClient = new CdClientContext();
                var id = Id;
                return cdClient.ObjectsTable.FirstOrDefault(o => o.Id == id);
            }
        }

        public int GetComponentId(ComponentId componentType)
        {
            return GetComponentId((int) componentType);
        }

        public int GetComponentId(int componentType)
        {
            var id = Id;
            using var cdClient = new CdClientContext();
            var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                r => r.Id == id && r.Componenttype == componentType
            );

            return itemRegistryEntry?.Componentid ?? 0;
        }

        public int[] GetComponentIds(ComponentId componentType)
        {
            return GetComponentIds((int) componentType);
        }

        public int[] GetComponentIds(int componentType)
        {
            var id = Id;

            using var cdClient = new CdClientContext();

            var itemRegistryEntry = cdClient.ComponentsRegistryTable.Where(
                r => r.Id == id && r.Componenttype == componentType
            );

            return itemRegistryEntry.Select(r => r.Componentid.Value).ToArray();
        }

        #region Consts

        public static readonly Lot ModularRocket = 6416;

        public static readonly Lot Spawner = 176;

        public const int Imagination = 935;
        public const int TwoImagination = 4035;
        public const int ThreeImagination = 11910;
        public const int FiveImagination = 11911;
        public const int TenImagination = 11918;
        public const int Health = 177;
        public const int TwoHealth = 11915;
        public const int ThreeHealth = 11916;
        public const int FiveHealth = 11917;
        public const int TenHealth = 11920;
        public const int Armor = 6431;
        public const int TwoArmor = 11912;
        public const int ThreeArmor = 11913;
        public const int FiveArmor = 11914;
        public const int TenArmor = 11919;

        public static readonly Lot RocketBuildNosecone = 6904;
        public static readonly Lot RocketBuildCockpit = 6910;
        public static readonly Lot RocketBuildTail = 6915;

        #endregion
    }
}