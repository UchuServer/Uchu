using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ScriptComponent")]
	public class ScriptComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("script_name")]
		public string Scriptname { get; set; }

		[Column("client_script_name")]
		public string Clientscriptname { get; set; }
	}
}