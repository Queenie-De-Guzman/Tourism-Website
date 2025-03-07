
	using System;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
namespace Try.Models
{
	public class Feedback
	{
		[Key]
		public int FeedbackID { get; set; }

		public int? DestinationID { get; set; }  // Foreign key reference

		[Required]
		[Column(TypeName = "nvarchar(max)")] // Use nvarchar(max) instead of deprecated text
		public string Message { get; set; }

		[Required]
		public DateTime SubmittedAt { get; set; } = DateTime.Now;

		// Navigation property (optional)
		// public virtual User User { get; set; }
	}
}