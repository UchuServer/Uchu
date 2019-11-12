using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.Client
{
	[Table("WhatsCoolNewsAndTips")]
	public class WhatsCoolNewsAndTips
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("iconID")]
		public int? IconID { get; set; }

		[Column("type")]
		public int? Type { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }
	}
}