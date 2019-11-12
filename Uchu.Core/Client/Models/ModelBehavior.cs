using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("ModelBehavior")]
	public class ModelBehavior
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("definitionXMLfilename")]
		public string DefinitionXMLfilename { get; set; }
	}
}