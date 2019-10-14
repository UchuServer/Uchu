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
            switch (obj)
            {
                case int i:
                    return i == Id;
                case Lot l:
                    return l.Id == Id;
                default:
                    return false;
            }
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
                using (var cdClient = new CdClientContext())
                {
                    var id = Id;
                    return cdClient.ObjectsTable.FirstOrDefault(o => o.Id == id);
                }
            }
        }

        public int GetComponentId(ComponentId componentType)
        {
            return GetComponentId((int) componentType);
        }

        public int GetComponentId(int componentType)
        {
            var id = Id;
            using (var cdClient = new CdClientContext())
            {
                var itemRegistryEntry = cdClient.ComponentsRegistryTable.FirstOrDefault(
                    r => r.Id == id && r.Componenttype == componentType
                );

                return itemRegistryEntry?.Componentid ?? 0;
            }
        }

        public int[] GetComponentIds(ComponentId componentType)
        {
            return GetComponentIds((int) componentType);
        }

        public int[] GetComponentIds(int componentType)
        {
            var id = Id;
            using (var cdClient = new CdClientContext())
            {
                var itemRegistryEntry = cdClient.ComponentsRegistryTable.Where(
                    r => r.Id == id && r.Componenttype == componentType
                );

                return itemRegistryEntry.Select(r => r.Componentid.Value).ToArray();
            }
        }

        #region Consts

        public const int ModularRocket = 6416;

        #endregion
    }
}