using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("MinifigComponent")]
	public class MinifigComponent
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("head")]
		public int? Head { get; set; }

		[Column("chest")]
		public int? Chest { get; set; }

		[Column("legs")]
		public int? Legs { get; set; }

		[Column("hairstyle")]
		public int? Hairstyle { get; set; }

		[Column("haircolor")]
		public int? Haircolor { get; set; }

		[Column("chestdecal")]
		public int? Chestdecal { get; set; }

		[Column("headcolor")]
		public int? Headcolor { get; set; }

		[Column("lefthand")]
		public int? Lefthand { get; set; }

		[Column("righthand")]
		public int? Righthand { get; set; }

		[Column("eyebrowstyle")]
		public int? Eyebrowstyle { get; set; }

		[Column("eyesstyle")]
		public int? Eyesstyle { get; set; }

		[Column("mouthstyle")]
		public int? Mouthstyle { get; set; }
	}
}