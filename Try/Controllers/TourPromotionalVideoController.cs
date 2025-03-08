using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Try.Models;
using Try.Services;

namespace Try.Controllers
{
	public class TourPromotionalVideoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment) : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment = webHostEnvironment;
		private readonly ApplicationDbContext _context = context;

		public IActionResult Index()
		{
			var tourpromotionalvideos = _context.TourPromotionalVideos.ToList();
			return View(tourpromotionalvideos);
		}

		public IActionResult ListVideo()
		{
			var tourpromotionalvideos = _context.TourPromotionalVideos.ToList();
			return View(tourpromotionalvideos);
		}

		public async Task<IActionResult> Create()
		{
			ViewBag.Packages = new SelectList(await _context.TourPackages.ToListAsync(), "PackageID", "PackageName");
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create([Bind("VideoTitle,VideoFile,ThumbnailURL,Description,PackageID")] TourPromotionalVideos tourpromotionalvideos, IFormFile VideoFile)
		{
			if (ModelState.IsValid)
			{
				// Validate if PackageID exists
				bool packageExists = await _context.TourPackages.AnyAsync(p => p.PackageID == tourpromotionalvideos.PackageID);
				if (!packageExists)
				{
					ModelState.AddModelError("PackageID", "Invalid Package selected.");
					ViewBag.Packages = new SelectList(await _context.TourPackages.ToListAsync(), "PackageID", "PackageName");
					return View(tourpromotionalvideos);
				}

				// Handle Video Upload
				if (VideoFile != null && VideoFile.Length > 0)
				{
					string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "videos");
					Directory.CreateDirectory(uploadsFolder); // Ensure the directory exists

					string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(VideoFile.FileName);
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);

					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await VideoFile.CopyToAsync(fileStream);
					}

					tourpromotionalvideos.VideoFile = "/videos/" + uniqueFileName;
				}

				tourpromotionalvideos.UploadedAt = DateTime.Now;

				_context.Add(tourpromotionalvideos);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(ListVideo));
			}

			// Reload packages if there's an error
			ViewBag.Packages = new SelectList(await _context.TourPackages.ToListAsync(), "PackageID", "PackageName");
			return View(tourpromotionalvideos);
		}

		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var tourpromotionalvideos = await _context.TourPromotionalVideos.FindAsync(id);
			if (tourpromotionalvideos == null) return NotFound();

			ViewBag.Packages = new SelectList(_context.TourPackages, "PackageID", "PackageName", tourpromotionalvideos.PackageID);
			return View(tourpromotionalvideos);
		}

		// POST: Edit Video
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("VideoID,VideoTitle,Description,PackageID")] TourPromotionalVideos tourpromotionalvideos, IFormFile VideoFile)
		{
			if (id != tourpromotionalvideos.VideoID) return NotFound();

			var existingVideo = await _context.TourPromotionalVideos.FindAsync(id);
			if (existingVideo == null) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					// Update Video Title and Description
					existingVideo.VideoTitle = tourpromotionalvideos.VideoTitle;
					existingVideo.Description = tourpromotionalvideos.Description;
					existingVideo.PackageID = tourpromotionalvideos.PackageID;

					// Handle Video File Upload (Optional)
					if (VideoFile != null && VideoFile.Length > 0)
					{
						// Delete Old Video
						if (!string.IsNullOrEmpty(existingVideo.VideoFile))
						{
							var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, existingVideo.VideoFile.TrimStart('/'));
							if (System.IO.File.Exists(oldPath))
							{
								System.IO.File.Delete(oldPath);
							}
						}

						// Save New Video
						string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "videos");
						if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

						string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(VideoFile.FileName);
						string filePath = Path.Combine(uploadsFolder, uniqueFileName);

						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await VideoFile.CopyToAsync(fileStream);
						}

						existingVideo.VideoFile = "/videos/" + uniqueFileName;
					}

					_context.Update(existingVideo);
					await _context.SaveChangesAsync();

					return RedirectToAction(nameof(ListVideo));
				}
				catch (DbUpdateException)
				{
					ModelState.AddModelError("", "Error updating video. Please try again.");
				}
			}

			ViewBag.Packages = new SelectList(_context.TourPackages, "PackageID", "PackageName", tourpromotionalvideos.PackageID);
			return View(tourpromotionalvideos);
		}

		// GET: Delete Confirmation Page
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var tourpromotionalvideos = await _context.TourPromotionalVideos.FindAsync(id);
			if (tourpromotionalvideos == null) return NotFound();

			return View(tourpromotionalvideos);
		}
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var tourpromotionalvideos = await _context.TourPromotionalVideos.FindAsync(id);
			if (tourpromotionalvideos != null)
			{
				_context.TourPromotionalVideos.Remove(tourpromotionalvideos);
				await _context.SaveChangesAsync();
			}

			return View("DeleteConfirmed"); // Redirect to the confirmation page
		}


		private bool TourPromotionalVideoExists(int id)
		{
			return _context.TourPromotionalVideos.Any(e => e.VideoID == id);
		}
	}
}