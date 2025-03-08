using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Models
{
	public class TourPromotionalVideos
	{
		[Key]
		public int VideoID { get; set; }

		public int PackageID { get; set; }

		[Required]
		[StringLength(255)]
		public string? VideoTitle { get; set; }


		public string? VideoFile { get; set; }

		[StringLength(500)]
		public string? ThumbnailURL { get; set; }

		public string? Description { get; set; }

		[DataType(DataType.DateTime)]
		public DateTime UploadedAt { get; set; } = DateTime.Now;








	}
}
