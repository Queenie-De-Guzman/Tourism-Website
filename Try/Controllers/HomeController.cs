using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Try.Models;
using Try.Services;

public class HomeController : Controller
{
	private readonly ILogger<HomeController> _logger;
	private readonly ApplicationDbContext _context;

	public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
	{
		_context = context;
		_logger = logger;
	}

	public IActionResult Index()
	{
		var destinations = _context.Destinations.ToList(); // Ensure this is not null
		return View(destinations);
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}


	public IActionResult TourPackages() 
	{

		var tourpackages = _context.TourPackages.ToList(); // Ensure this is not null
		return View(tourpackages);
		
	}


	public IActionResult DestinationSpot()
	{

		var tourpackages = _context.TourPackages.ToList(); // Ensure this is not null
		return View(tourpackages);

	}
}
