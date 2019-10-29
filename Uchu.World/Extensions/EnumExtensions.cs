using System;
using System.Linq;
using Uchu.Core;

namespace Uchu.World
{
    public static class EnumExtensions
    {
        public static ZoneChecksum GetChecksum(this ZoneId @this)
        {
            var name = Enum.GetName(typeof(ZoneId), @this);

            var names = Enum.GetNames(typeof(ZoneChecksum));
            var values = Enum.GetValues(typeof(ZoneChecksum));

            return (ZoneChecksum) values.GetValue(names.ToList().IndexOf(name));
        }
        
        public static InventoryType GetInventoryType(this ItemType @this)
        {
            switch (@this)
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

                case ItemType.Invalid:
                    return InventoryType.Invalid;
                default:
                    Logger.Error(new ArgumentOutOfRangeException(nameof(@this), @this, null));
                    return InventoryType.Invalid;
            }
        }

        public static BehaviorSlot GetBehaviorSlot(this ItemType @this)
        {
            return @this switch
            {
                ItemType.Invalid => BehaviorSlot.Invalid,
                ItemType.Brick => BehaviorSlot.Invalid,
                ItemType.Legs => BehaviorSlot.Invalid,
                ItemType.Chest => BehaviorSlot.Invalid,
                ItemType.LeftTrinket => BehaviorSlot.Invalid,
                ItemType.RightTrinket => BehaviorSlot.Invalid,
                ItemType.Behavior => BehaviorSlot.Invalid,
                ItemType.Property => BehaviorSlot.Invalid,
                ItemType.Model => BehaviorSlot.Invalid,
                ItemType.Collectable => BehaviorSlot.Invalid,
                ItemType.PetFood => BehaviorSlot.Invalid,
                ItemType.Egg => BehaviorSlot.Invalid,
                ItemType.QuestObject => BehaviorSlot.Invalid,
                ItemType.PetInventoryItem => BehaviorSlot.Invalid,
                ItemType.T20Package => BehaviorSlot.Invalid,
                ItemType.LootModel => BehaviorSlot.Invalid,
                ItemType.Vehicle => BehaviorSlot.Invalid,
                ItemType.Currency => BehaviorSlot.Invalid,
                ItemType.Hair => BehaviorSlot.Invalid,
                ItemType.Hat => BehaviorSlot.Head,
                ItemType.LeftHand => BehaviorSlot.LeftHand,
                ItemType.RightHand => BehaviorSlot.Primary,
                ItemType.Consumable => BehaviorSlot.Consumeable,
                ItemType.Neck => BehaviorSlot.Neck,
                _ => throw new ArgumentOutOfRangeException(nameof(@this), @this, null)
            };
        }
    }
}