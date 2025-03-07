using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Models
{
	public class TourPackages
	{
		[Key]
		public int PackageID { get; set; } // Primary Key

		[ForeignKey("Destination")]
		public int? DestinationID { get; set; } // Foreign Key

		[Required]
		[StringLength(255)]
		public string PackageName { get; set; } = string.Empty;

		[Required]
		[Column(TypeName = "decimal(10,2)")]
		public decimal Price { get; set; }

		[Required]
		public int DurationDays { get; set; }

		[StringLength(500)]
		public string? ImagePath { get; set; } // Stores image URL or file path

		[Required]
		public DateTime StartDate { get; set; }

		[Required]
		public DateTime EndDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public DateTime CreatedAt { get; set; } = DateTime.Now;

		// Navigation property
		public virtual Destinations? Destination { get; set; }
	}
}
