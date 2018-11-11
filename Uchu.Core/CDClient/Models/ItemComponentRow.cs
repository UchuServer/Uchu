using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class ItemComponentRow
    {
        [Column("id")]
        public int ItemId { get; set; }

        [Column("equipLocation")]
        public string EquipLocation { get; set; }

        [Column("baseValue")]
        public int BaseValue { get; set; }

        [Column("isKitPiece")]
        public bool IsKitPiece { get; set; }

        [Column("rarity")]
        public int Rarity { get; set; }

        [Column("itemType")]
        public int ItemType { get; set; }

        [Column("inLootTable")]
        public bool IsInLootTable { get; set; }

        [Column("inVendor")]
        public bool IsInVendor { get; set; }

        [Column("isUnique")]
        public bool IsUnique { get; set; }

        [Column("isBOP")]
        public bool isBOP { get; set; }

        [Column("isBOE")]
        public bool isBOE { get; set; }

        [Column("reqFlagID")]
        public int RequiredFlagId { get; set; }

        [Column("reqSpecialtyID")]
        public int RequiredSpecialtyId { get; set; }

        [Column("reqSpecRank")]
        public int RequiredSpecialtyRank { get; set; }

        [Column("reqAchievementID")]
        public int RequiredAchievementId { get; set; }

        [Column("stackSize")]
        public int StackSize { get; set; }

        [Column("color1")]
        public int Color { get; set; }

        [Column("decal")]
        public int Decal { get; set; }

        [Column("offsetGroupID")]
        public int OffsetGroupId { get; set; }

        [Column("buildTypes")]
        public int BuildTypes { get; set; }

        [Column("reqPrecondition")]
        public string RequiredPrecondition { get; set; }

        [Column("animationFlag")]
        public int AnimationFlag { get; set; }

        [Column("equipEffects")]
        public int EquipEffects { get; set; }

        [Column("readyForQA")]
        public bool ReadyForQA { get; set; }

        [Column("itemRating")]
        public int ItemRating { get; set; }

        [Column("isTwoHanded")]
        public bool IsTwoHanded { get; set; }

        [Column("minNumRequired")]
        public int MinimumNumberRequired { get; set; }

        [Column("delResIndex")]
        public int DelResIndex { get; set; } // what does this mean?

        [Column("currencyLOT")]
        public int AmmunitionLOT { get; set; } // I guess this is ammo instead of currency?

        [Column("altCurrencyCost")]
        public int AltAmmunitionCost { get; set; }

        [Column("noEquipAnimation")]
        public bool NoEquipAnimation { get; set; }

        [Column("commendationLOT")]
        public int CommendationLOT { get; set; }

        [Column("commendationCost")]
        public int CommendationCost { get; set; }

        [Column("audioEquipMetaEventSet")]
        public string AudioEquipMetaEventSet { get; set; }

        [Column("currencyCosts")]
        public string CurrencyCosts { get; set; }

        [Column("ingredientInfo")]
        public string IngredientInfo { get; set; }

        [Column("locStatus")]
        public int LocStatus { get; set; }

        [Column("forgeType")]
        public int ForgeType { get; set; }

        [Column("SellMultiplier")]
        public float SellMultiplier { get; set; }
    }
}