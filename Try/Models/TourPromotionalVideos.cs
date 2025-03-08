using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Models
{
	public class TourPromotionalVideos
	{
		[Key]
		public int VideoID { get; set; }

		[Required]
		public int TourID { get; set; }

		[Required]
		[StringLength(255)]
		public string? VideoTitle { get; set; }

	
		public string? VideoURL { get; set; }

		[StringLength(500)]
		public string ?ThumbnailURL { get; set; }

		public string? Description { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime UploadedAt { get; set; } = DateTime.Now;
	

		// Navigation Property
		[ForeignKey("TourID")]
		public virtual TourPackages TourPackages { get; set; }





	}
}
