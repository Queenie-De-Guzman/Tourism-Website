using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Models
{

	public class Destinations
	{
		[Key]
		public int DestinationID { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[StringLength(255)]
		public string Location { get; set; }

		public string Description { get; set; }

		[StringLength(500)]
		public string? ImageURL { get; set; }

		[NotMapped] // Prevents it from being stored in the database
		public IFormFile? ImageURLFile { get; set; } // This will handle file uploads

		[Required]
		public DateTime CreatedAt { get; set; } = DateTime.Now;
	}
}

