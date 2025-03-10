using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try.Models;
using Try.Services;


namespace Try.Controllers
{
	public class BlogPostsController : Controller
	{
		private readonly ApplicationDbContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public BlogPostsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

		public IActionResult Index()
		{
			var blogposts = _context.BlogPosts.ToList();
			return View(blogposts);
		}

		public IActionResult Create()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(BlogPosts blogposts, IFormFile imageFile)
		{
			if (ModelState.IsValid)
			{
				if (imageFile != null)
				{
					string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
					string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						imageFile.CopyTo(fileStream); // FIXED: Using imageFile instead of blogposts
					}

					blogposts.imageFile = "/images/" + uniqueFileName; // Save the relative path
				}

				_context.BlogPosts.Add(blogposts);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(blogposts);
		}


		public IActionResult Edit(int id)
		{
			var blogposts = _context.BlogPosts.Find(id);
			if (blogposts == null)
			{
				return NotFound();
			}
			return View(blogposts);
		}

		[HttpPost]
		public IActionResult Edit(int id, BlogPosts blogposts, IFormFile? imageFile)
	{
			if (blogposts == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				blogposts.title = blogposts.title;
				blogposts.location = blogposts.location;
				blogposts.date = blogposts.date;
				blogposts.description = blogposts.description;
				if (imageFile != null)
				{
					// Delete old image if exists
					if (!string.IsNullOrEmpty(blogposts.imageFile))
					{
						string oldPath = Path.Combine(_webHostEnvironment.WebRootPath, blogposts.imageFile.TrimStart('/'));
						if (System.IO.File.Exists(oldPath))
						{
							System.IO.File.Delete(oldPath);
						}
					}

					// Save new image
					string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
					string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						imageFile.CopyTo(fileStream);
					}

					blogposts.imageFile = "/images/" + uniqueFileName; // Save relative path
				}

				_context.BlogPosts.Update(blogposts);
				_context.SaveChanges();
				return RedirectToAction("Index");
			}
			return View(blogposts);
		}


	}
}