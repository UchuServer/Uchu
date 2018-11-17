using System;
using System.Linq;

namespace Uchu.Core
{
    public static class Utils
    {
        private static readonly Random Random = new Random();

        public static long RandomLong(long min, long max)
        {
            var buf = new byte[8];

            Random.NextBytes(buf);

            var res = BitConverter.ToInt64(buf, 0);

            return Math.Abs(res % (max - min)) + min;
        }

        public static long GenerateObjectId()
            => RandomLong(1000000000000000000, 1999999999999999999);

        public static long GenerateSpawnerId()
            => RandomLong(70000000000000, 79999999999999);

        public static ZoneChecksum GetChecksum(ZoneId id)
        {
            var name = Enum.GetName(typeof(ZoneId), id);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }

        public static InventoryType GetItemInventoryType(ItemType type)
        {
            switch (type)
            {
                case ItemType.Behavior:
                    return InventoryType.Behaviors;

                case ItemType.Brick:
                    return InventoryType.Bricks;

                case ItemType.Property:
                    return InventoryType.PropertyDeeds;

                case ItemType.Model:
                case ItemType.LootModel:
                case ItemType.Vehicle:
                    return InventoryType.Models;

                case ItemType.Hat:
                case ItemType.Hair:
                case ItemType.Neck:
                case ItemType.Chest:
                case ItemType.LeftHand:
                case ItemType.RightHand:
                case ItemType.LeftTrinket:
                case ItemType.RightTrinket:
                case ItemType.Legs:
                case ItemType.Collectable:
                case ItemType.Consumable:
                case ItemType.Egg:
                case ItemType.PetFood:
                case ItemType.T20Package:
                case ItemType.PetInventoryItem:
                case ItemType.Currency:
                    return InventoryType.Items;

                case ItemType.QuestObject:
                    return InventoryType.Hidden;

                default:
                    return InventoryType.Invalid;
            }
        }
    }
}