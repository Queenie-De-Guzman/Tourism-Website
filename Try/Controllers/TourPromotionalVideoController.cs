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






