using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try.Models;
using Try.Services;


namespace Try.Controllers
{

	public class DestinationController : Controller
	{
		private readonly ApplicationDbContext _context;

		public DestinationController(ApplicationDbContext context)
		{
			_context = context;
		}
		// Check if current user is Admin
		private bool IsAdmin()
		{
			return HttpContext.Session.GetString("Role") == "Admin";
		}

		public IActionResult Destinations()
		{
			var destinations = _context.Destinations.ToList(); // Ensure you're fetching Destinations
			return View(destinations);
		}

		// CREATE: Show form
		public IActionResult Create()
		{
			return View();
		}


		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Destinations destinations, IFormFile ImageURL)
		{
			if (ModelState.IsValid)
			{
				if (ImageURL != null && ImageURL.Length > 0)
				{
					var fileName = Path.GetFileName(ImageURL.FileName);
					var filePath = Path.Combine("wwwroot/images", fileName);

					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						await ImageURL.CopyToAsync(stream);
					}

					destinations.ImageURL = "/images/" + fileName;
				}

				_context.Destinations.Add(destinations);
				await _context.SaveChangesAsync();
				return RedirectToAction(nameof(Destinations));
			}
			return View(destinations);
		}


		// READ: Get Single Destination Details
		public async Task<IActionResult> Details(int? id)
		{
			if (id == null) return NotFound();

			var destinations = await _context.Destinations
				.FirstOrDefaultAsync(m => m.DestinationID == id);

			if (destinations == null) return NotFound();

			return View(destinations);
		}

		public async Task<IActionResult> Edit(int id)
		{
			var destinations = await _context.Destinations.FindAsync(id);
			if (destinations == null)
			{
				return NotFound();
			}
			return View(destinations);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Destinations destinations, IFormFile ImageURL)
		{
			if (id != destinations.DestinationID)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (ImageURL != null && ImageURL.Length > 0)
					{
						var fileName = Path.GetFileName(ImageURL.FileName);
						var filePath = Path.Combine("wwwroot/images", fileName);

						using (var stream = new FileStream(filePath, FileMode.Create))
						{
							await ImageURL.CopyToAsync(stream);
						}

						destinations.ImageURL = "/images/" + fileName;
					}

					_context.Update(destinations);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_context.Destinations.Any(e => e.DestinationID == id))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(Destinations));
			}
			return View(destinations);
		}

		public async Task<IActionResult> Delete(int id)
		{
			var destinations = await _context.Destinations.FindAsync(id);
			if (destinations == null)
			{
				return NotFound();
			}
			return View(destinations);
		}

		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var destinations = await _context.Destinations.FindAsync(id);
			if (destinations != null)
			{
				// Delete the image file if it exists
				if (!string.IsNullOrEmpty(destinations.ImageURL))
				{
					var filePath = Path.Combine("wwwroot", destinations.ImageURL.TrimStart('/'));
					if (System.IO.File.Exists(filePath))
					{
						System.IO.File.Delete(filePath);
					}
				}

				_context.Destinations.Remove(destinations);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(Destinations));
		}

	}
}
