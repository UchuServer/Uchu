using System;
using Uchu.Core;

namespace Uchu.World
{
    public enum ItemType
    {
        Invalid,
        Brick,
        Hat,
        Hair,
        Neck,
        LeftHand,
        RightHand,
        Legs,
        LeftTrinket,
        RightTrinket,
        Behavior,
        Property,
        Model,
        Collectable,
        Consumable,
        Chest,
        Egg,
        PetFood,
        QuestObject,
        PetInventoryItem,
        T20Package,
        LootModel,
        Vehicle,
        Currency
    }

    public static class ItemTypeExtensions
    {
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
            switch (@this)
            {
                case ItemType.Invalid:
                case ItemType.Brick:
                case ItemType.Legs:
                case ItemType.Chest:
                case ItemType.LeftTrinket:
                case ItemType.RightTrinket:
                case ItemType.Behavior:
                case ItemType.Property:
                case ItemType.Model:
                case ItemType.Collectable:
                case ItemType.PetFood:
                case ItemType.Egg:
                case ItemType.QuestObject:
                case ItemType.PetInventoryItem:
                case ItemType.T20Package:
                case ItemType.LootModel:
                case ItemType.Vehicle:
                case ItemType.Currency:
                case ItemType.Hair:
                    return BehaviorSlot.Invalid;
                case ItemType.Hat:
                    return BehaviorSlot.Head;
                case ItemType.LeftHand:
                    return BehaviorSlot.LeftHand;
                case ItemType.RightHand:
                    return BehaviorSlot.Primary;
                case ItemType.Consumable:
                    return BehaviorSlot.Consumeable;
                case ItemType.Neck:
                    return BehaviorSlot.Neck;
                default:
                    throw new ArgumentOutOfRangeException(nameof(@this), @this, null);
            }
        }
    }
}