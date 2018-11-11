using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uchu.Core
{
    public class ComponentsRegistryRow
    {
        [Column("id")]
        public int LOT { get; set; }

        [Column("component_type")]
        public int ComponentType { get; set; }

        [Column("component_id")]
        public int ComponentId { get; set; }
    }
}