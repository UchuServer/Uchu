using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class SkillBehaviorRow
    {
        [Column("skillID")]
        public int SkillId { get; set; }

        [Column("locStatus")]
        public int LocStatus { get; set; }

        [Column("behaviorID")]
        public int BehaviorId { get; set; }

        [Column("imaginationcost")]
        public int ImaginationCost { get; set; }

        [Column("cooldowngroup")]
        public int CooldownGroup { get; set; }

        [Column("cooldown")]
        public float Cooldown { get; set; }

        [Column("inNpcEditor")]
        public bool InNPCEditor { get; set; }

        [Column("skillIcon")]
        public int SkillIcon { get; set; }

        [Column("oomSkillID")]
        public string OOMSkillId { get; set; }

        [Column("oomBehaviorEffectID")]
        public int OOMBehaviorEffectId { get; set; }

        [Column("castTypeDesc")]
        public int CastTypeDesc { get; set; }

        [Column("imBonusUI")]
        public int ImaginationBonusUserInterface { get; set; }

        [Column("lifeBonusUI")]
        public int LifeBonusUserInterface { get; set; }

        [Column("armorBonusUI")]
        public int ArmorBonusUserInterface { get; set; }

        [Column("damageUI")]
        public int DamageUserInterface { get; set; }

        [Column("hideIcon")]
        public bool HideIcon { get; set; }

        [Column("localize")]
        public bool Localize { get; set; }

        [Column("gate_version")]
        public string GateVersion { get; set; }

        [Column("cancelType")]
        public int CancelType { get; set; }
    }
}