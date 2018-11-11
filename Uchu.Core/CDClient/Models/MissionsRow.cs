using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class MissionsRow
    {
        [Column("id"), Key]
        public int MissionId { get; set; }

        [Column("defined_type")]
        public string Type { get; set; }

        [Column("defined_subtype")]
        public string Subtype { get; set; }

        [Column("UISortOrder")]
        public int UserInterfaceSortOrder { get; set; }

        [Column("offer_objectID")]
        public int OffererObjectId { get; set; }

        [Column("target_objectId")]
        public int TargetObjectId { get; set; }

        [Column("reward_currency")]
        public long CurrencyReward { get; set; }

        [Column("LegoScore")]
        public int LegoScoreReward { get; set; }

        [Column("reward_reputation")]
        public long ReputationReward { get; set; }

        [Column("isChoiceReward")]
        public bool IsChoiceReward { get; set; }

        [Column("reward_item1")]
        public int FirstItemReward { get; set; }

        [Column("reward_item1_count")]
        public int FirstItemRewardCount { get; set; }

        [Column("reward_item2")]
        public int SecondItemReward { get; set; }

        [Column("reward_item2_count")]
        public int SecondItemRewardCount { get; set; }

        [Column("reward_item3")]
        public int ThirdItemReward { get; set; }

        [Column("reward_item3_count")]
        public int ThirdItemRewardCount { get; set; }

        [Column("reward_item4")]
        public int FourthItemReward { get; set; }

        [Column("reward_item4_count")]
        public int FourthItemRewardCount { get; set; }

        [Column("reward_emote")]
        public int FirstEmoteReward { get; set; }

        [Column("reward_emote2")]
        public int SecondEmoteReward { get; set; }

        [Column("reward_emote3")]
        public int ThirdEmoteReward { get; set; }

        [Column("reward_emote4")]
        public int FourthEmoteReward { get; set; }

        [Column("reward_maximagination")]
        public int MaximumImaginationReward { get; set; }

        [Column("reward_maxhealth")]
        public int MaximumHealthReward { get; set; }

        [Column("reward_maxinventory")]
        public int MaximumInventoryReward { get; set; }

        [Column("reward_maxmodel")]
        public int MaximumModelsReward { get; set; }

        [Column("reward_maxwidget")]
        public int MaximumWidgetsReward { get; set; }

        [Column("reward_maxwallet")]
        public long MaximumWalletReward { get; set; }

        [Column("repeatable")]
        public bool IsRepeatable { get; set; }

        [Column("reward_currency_repeatable")]
        public long RepeatableCurrencyReward { get; set; }

        [Column("reward_item1_repeatable")]
        public int FirstRepeatableItemReward { get; set; }

        [Column("reward_item1_repeat_count")]
        public int FirstRepeatableItemRewardCount { get; set; }

        [Column("reward_item2_repeatable")]
        public int SecondRepeatableItemReward { get; set; }

        [Column("reward_item2_repeat_count")]
        public int SecondRepeatableItemRewardCount { get; set; }

        [Column("reward_item3_repeatable")]
        public int ThirdRepeatableItemReward { get; set; }

        [Column("reward_item3_repeat_count")]
        public int ThirdRepeatableItemRewardCount { get; set; }

        [Column("reward_item4_repeatable")]
        public int FourthRepeatableItemReward { get; set; }

        [Column("reward_item4_repeat_count")]
        public int FourthRepeatableItemRewardCount { get; set; }

        [Column("time_limit")]
        public int TimeLimit { get; set; }

        [Column("isMission")]
        public bool IsMission { get; set; }

        [Column("missionIconID")]
        public int MissionIconId { get; set; }

        [Column("prereqMissionID")]
        public int[] PrerequiredMissions { get; set; }

        [Column("localize")]
        public bool Localize { get; set; }

        [Column("inMOTD")]
        public bool IsInMotd { get; set; }

        [Column("cooldownTime")]
        public long CooldownTime { get; set; }

        [Column("isRandom")]
        public bool IsRandom { get; set; }

        [Column("randomPool")]
        public string RandomPool { get; set; }

        [Column("UIPrereqID")]
        public int PrerequiredUserInterfaceId { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }

        [Column("HUDStates")]
        public string HUDStates { get; set; }

        [Column("locStatus")]
        public int LocStatus { get; set; }

        [Column("reward_bankinventory")]
        public int BankInventoryReward { get; set; }
    }
}