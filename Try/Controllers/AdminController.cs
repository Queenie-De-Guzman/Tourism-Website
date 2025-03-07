using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Try.Services;
using Try.Models;
using Microsoft.AspNetCore.Hosting;


namespace Try.Controllers
{

	
	public class AdminController : Controller
    {
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly ApplicationDbContext _context;

		public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}
		public IActionResult Index()
        {
            return View();
        }
		public IActionResult Dashboard()
		{
			if (HttpContext.Session.GetString("Role") != "Admin")
			{
				return RedirectToAction("Login", "User");
			}
			return View();
		}

		public IActionResult Inquiries()
		{
			var inquiries = _context.Inquiries.OrderByDescending(i => i.InquiryDate).ToList();
			return View(inquiries);
		}

		[HttpPost]
		public IActionResult DeleteInquiry(int id)
		{
			var inquiry = _context.Inquiries.Find(id);
			if (inquiry != null)
			{
				_context.Inquiries.Remove(inquiry);
				_context.SaveChanges();
			}
			return RedirectToAction("Inquiries");
		}
		
		
		
		// Feedback

		public IActionResult Feedback()
		{
			var feedback= _context.Feedback.OrderByDescending(f => f.SubmittedAt).ToList();
			return View(feedback);
		}
		[HttpPost]
		public IActionResult DeleteFeedback(int id)
		{
			var feedback = _context.Feedback.Find(id);
			if (feedback != null)
			{
				_context.Feedback.Remove(feedback);
				_context.SaveChanges();
			}
			return RedirectToAction("Feedback");
		}

		//TourPackages


		// GET: Admin Panel - List All Packages
		public async Task<IActionResult> TourPackages()
		{
			var tourpackages = await _context.TourPackages.Include(t => t.Destination).ToListAsync();
			return View(tourpackages);
		}
		// GET: Admin Panel - Create New Package
		// GET: TourPackages/Create
		public IActionResult Create()
		{
			return View();
		}

		// POST: TourPackages/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(TourPackages tourpackges, IFormFile ImagePath)
		{
			if (ModelState.IsValid)
			{
				if (ImagePath != null)
				{
					// Define the path where the file will be saved
					string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
					string uniqueFileName = Guid.NewGuid().ToString() + "_" + ImagePath.FileName;
					string filePath = Path.Combine(uploadsFolder, uniqueFileName);

					// Ensure the folder exists
					if (!Directory.Exists(uploadsFolder))
					{
						Directory.CreateDirectory(uploadsFolder);
					}

					// Save the uploaded file to the server
					using (var fileStream = new FileStream(filePath, FileMode.Create))
					{
						await ImagePath.CopyToAsync(fileStream);
					}

					// Set the image path in the model
					tourpackges.ImagePath = "/images/" + uniqueFileName;
				}

				// TODO: Save the model to the database
				_context.TourPackages.Add(tourpackges);
				 await _context.SaveChangesAsync();

				return RedirectToAction("Dashboard");
			}

			return View(tourpackges);
		}

		// GET: TourPackages/Edit/5
		public async Task<IActionResult> Edit(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}

			var tourpackages = await _context.TourPackages.FindAsync(id);
			if (tourpackages == null)
			{
				return NotFound();
			}

			return View(tourpackages);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(int id, [Bind("PackageID,PackageName,Price,DurationDays,StartDate,EndDate")] TourPackages tourpackages, IFormFile? ImagePath)
		{
			if (id != tourpackages.PackageID)
			{
				return NotFound();
			}

			var existingPackage = await _context.TourPackages.AsNoTracking().FirstOrDefaultAsync(p => p.PackageID == id);
			if (existingPackage == null)
			{
				return NotFound();
			}

			if (ModelState.IsValid)
			{
				try
				{
					if (ImagePath != null)
					{
						// Save new image
						string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
						string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagePath.FileName);
						string filePath = Path.Combine(uploadsFolder, uniqueFileName);

						using (var fileStream = new FileStream(filePath, FileMode.Create))
						{
							await ImagePath.CopyToAsync(fileStream);
						}

						// Update image path
						tourpackages.ImagePath = "/images/" + uniqueFileName;
					}
					else
					{
						// Retain existing image if no new image is uploaded
						tourpackages.ImagePath = existingPackage.ImagePath;
					}

					_context.Update(tourpackages);
					await _context.SaveChangesAsync();
				}
				catch (DbUpdateConcurrencyException)
				{
					if (!TourPackageExists(tourpackages.PackageID))
					{
						return NotFound();
					}
					else
					{
						throw;
					}
				}
				return RedirectToAction(nameof(TourPackages));
			}
			return View(tourpackages);
		}


		private bool TourPackageExists(int id)
		{
			return _context.TourPackages.Any(e => e.PackageID == id);
		}



	}
}
