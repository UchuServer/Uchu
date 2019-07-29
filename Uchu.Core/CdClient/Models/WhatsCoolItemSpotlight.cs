using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Uchu.Core.CdClient
{
	[Table("WhatsCoolItemSpotlight")]
	public class WhatsCoolItemSpotlight
	{
		[Key] [Column("efId")]
		public int EfId { get; set; }

		[Column("id")]
		public int? Id { get; set; }

		[Column("itemID")]
		public int? ItemID { get; set; }

		[Column("localize")]
		public bool? Localize { get; set; }

		[Column("gate_version")]
		public string Gateversion { get; set; }

		[Column("locStatus")]
		public int? LocStatus { get; set; }
	}
}