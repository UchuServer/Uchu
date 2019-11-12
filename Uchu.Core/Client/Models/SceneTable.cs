using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("SceneTable")]
	public class SceneTable
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("sceneID")]
		public int? SceneID { get; set; }

		[Column("sceneName")]
		public string SceneName { get; set; }
	}
}