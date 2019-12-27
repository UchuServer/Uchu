using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Uchu.Core
{
    public class MissionTask
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public List<MissionTaskValue> Values { get; set; }

        public int MissionId { get; set; }

        [ForeignKey(nameof(MissionId))]
        public Mission Mission { get; set; }

        public float[] ValueArray() => Values.SelectMany(v => Enumerable.Repeat(v.Value, v.Count)).ToArray();

        public void Add(float value)
        {
            foreach (var taskValue in Values.Where(taskValue => taskValue.Value.Equals(value)))
            {
                taskValue.Count++;
                    
                return;
            }

            Values.Add(new MissionTaskValue
            {
                Value = value,
                Count = 1
            });
        }

        public void Remove(float value)
        {
            foreach (var taskValue in Values.Where(taskValue => taskValue.Value.Equals(value)).ToArray())
            {
                if (--taskValue.Count == 0)
                {
                    Values.Remove(taskValue);
                }
                    
                return;
            }
        }

        public bool Contains(float value) => Values.Any(v => v.Value.Equals(value));
    }
}