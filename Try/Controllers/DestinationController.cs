using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Try.Models;
using Try.Services;


namespace Destination.Controllers
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

		// CREATE: Save form data
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(Destinations destinations)
		{
			if (ModelState.IsValid)
			{
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

		// UPDATE: Show edit form
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null) return NotFound();

			var destinations = await _context.Destinations.FindAsync(id);
			if (destinations == null) return NotFound();

			return View(destinations);
		}

		// UPDATE: Save edited data
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, Destinations destinations)
		{
			if (id != destinations.DestinationID) return NotFound();

			if (ModelState.IsValid)
			{
				try
				{
					_context.Update(destinations);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!_context.Destinations.Any(e => e.DestinationID == id))
						return NotFound();
					else
						throw;
				}
				return RedirectToAction(nameof(Destinations));
			}
			return View(destinations);
		}

		// DELETE: Show confirmation page
		public async Task<IActionResult> Delete(int? id)
		{
			if (id == null) return NotFound();

			var destinations = await _context.Destinations
				.FirstOrDefaultAsync(m => m.DestinationID == id);

			if (destinations == null) return NotFound();

			return View(destinations);
		}

		// DELETE: Confirm and remove from database
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteConfirmed(int id)
		{
			var destinations = await _context.Destinations.FindAsync(id);
			if (destinations != null)
			{
				_context.Destinations.Remove(destinations);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction(nameof(destinations));
		}
	}
}
