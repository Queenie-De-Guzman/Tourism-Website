using System.ComponentModel.DataAnnotations;
using System;

namespace Try.Models
{
	public class BlogPosts
	{
		[Key]
		public int id { get; set; }

		[Required]
		[StringLength(255)]
		public string title { get; set; }

		[StringLength(255)]
		public string location { get; set; }

		[Required]
		public DateTime date { get; set; }

		public string description { get; set; }

		[StringLength(255)] // Store image path or URL
		public string imageFile { get; set; }
	}

}

