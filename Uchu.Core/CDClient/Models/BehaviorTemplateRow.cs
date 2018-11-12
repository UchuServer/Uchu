using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class BehaviorTemplateRow
    {
        [Column("behaviorID")]
        public int BehaviorId { get; set; }

        [Column("templateID")]
        public int TemplateId { get; set; }

        [Column("effectID")]
        public int EffectId { get; set; }

        [Column("effectHandle")]
        public string EffectHandle { get; set; }
    }
}