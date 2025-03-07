using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Try.Models
{
	public class Inquiries
	{
		[Key]
		public int InquiryID { get; set; }

		public int? Id { get; set; }

		[Required]
		[StringLength(100)]
		public string Name { get; set; }

		[Required]
		[StringLength(100)]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		public string Message { get; set; }

		[Required]
		public DateTime InquiryDate { get; set; }
	}
}
