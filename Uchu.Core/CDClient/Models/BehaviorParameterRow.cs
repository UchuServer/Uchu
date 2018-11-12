using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class BehaviorParameterRow
    {
        [Column("behaviorID")]
        public int BehaviorId { get; set; }

        [Column("parameterID")]
        public string ParameterId { get; set; }

        [Column("value")]
        public float Value { get; set; }
    }
}