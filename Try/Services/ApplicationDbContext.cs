using Microsoft.EntityFrameworkCore;
using Try.Models;


namespace Try.Services
{
	public class ApplicationDbContext : DbContext
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }


		public DbSet<Users> Users { get; set; }
		public DbSet<Inquiries> Inquiries { get; set; }

		public DbSet<Feedback> Feedback { get; set; }
		public DbSet<TourPackages> TourPackages { get; set; }
		public DbSet<Destinations> Destinations { get; set; }
		public DbSet<TourPromotionalVideos> TourPromotionalVideos { get; set; }
		public DbSet<BlogPosts> BlogPosts { get; set; }
	}
}