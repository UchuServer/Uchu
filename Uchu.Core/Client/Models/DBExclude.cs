using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("DBExclude")]
	public class DBExclude
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("table")]
		public string Table { get; set; }

		[Column("column")]
		public string Column { get; set; }
	}
}