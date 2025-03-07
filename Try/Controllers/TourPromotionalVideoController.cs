using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try.Models;
using Try.Services;

namespace Try.Controllers
{
	public class TourPromotionalVideoController : Controller
	{
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ApplicationDbContext _context;

		public TourPromotionalVideoController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}

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

// GET: Create Video Form
public IActionResult Create()
{
    return View();
}

// POST: TourPromotionalVideos/Create
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(TourPromotionalVideos tourpromotionalvideos)
{
    if (ModelState.IsValid)
    {
        if (tourpromotionalvideos.VideoFile != null)
        {
            var allowedExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv" };
            var fileExtension = Path.GetExtension(tourpromotionalvideos.VideoFile.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                ModelState.AddModelError("VideoFile", "Invalid file format. Only MP4, AVI, MOV, and WMV are allowed.");
                return View(tourpromotionalvideos);
            }

            if (tourpromotionalvideos.VideoFile.Length > 50 * 1024 * 1024) // Limit to 50MB
            {
                ModelState.AddModelError("VideoFile", "File size exceeds the 50MB limit.");
                return View(tourpromotionalvideos);
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/videos");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await tourpromotionalvideos.VideoFile.CopyToAsync(stream);
            }

            tourpromotionalvideos.VideoURL = "/videos/" + uniqueFileName;
        }

        _context.Add(tourpromotionalvideos);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    return View(tourpromotionalvideos);
}

			 // GET: TourPromotionalVideos/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var tourpromotionalvideos = await _context.TourPromotionalVideos.FindAsync(id);
			if (tourpromotionalvideos == null) return NotFound();

			return View(tourpromotionalvideos);
		}

		// POST: TourPromotionalVideos/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, TourPromotionalVideos tourpromotionalvideos)
		{
			if (id != tourpromotionalvideos.VideoID) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(tourpromotionalvideos);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!VideoExists(tourpromotionalvideos.VideoID)) return NotFound();
					else throw;
				}
				return RedirectToAction(nameof(Index));
			}
			return View(tourpromotionalvideos);
		}
		private bool VideoExists(int id)
		{
			return _context.TourPromotionalVideos.Any(e => e.VideoID == id);
		}

	}
}






