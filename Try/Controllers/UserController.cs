using Try.Services;
using Try.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Try.Controllers
{
    public class UserController : Controller
    {
		private readonly ApplicationDbContext _context;

		public UserController(ApplicationDbContext context)
		{
			_context = context;
		}
		public IActionResult Index()
        {
            return View();
        }


		// GET: User/Login
		public IActionResult Login()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Login(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				ModelState.AddModelError("", "Username and password are required.");
				return View();
			}

			string hashedPassword = HashPassword(password);
			var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedPassword);

			if (user == null)
			{
				ModelState.AddModelError("", "Invalid username or password.");
				return View();
			}

			// Create user claims
			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, user.Username),
		new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
		new Claim(ClaimTypes.Role, user.Role)
	};

			var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
			var authProperties = new AuthenticationProperties { IsPersistent = true };

			// Sign in user
			await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
				new ClaimsPrincipal(claimsIdentity), authProperties);

			return user.Role == "Admin"
				? RedirectToAction("Dashboard", "Admin")
				: RedirectToAction("Dashboard", "User");
		}


		// GET: User/Register
		public IActionResult Register()
		{
			return View();
		}

		// POST: User/Register
		[HttpPost]
		public IActionResult Register(string username, string password)
		{
			if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
			{
				ModelState.AddModelError("", "Username and password are required.");
				return View();
			}

			if (_context.Users.Any(u => u.Username == username))
			{
				ModelState.AddModelError("", "Username is already taken. Please choose another.");
				return View();
			}

			string hashedPassword = HashPassword(password);

			var newUser = new Users
			{
				Username = username,
				PasswordHash = hashedPassword,
				Role = _context.Users.Any() ? "User" : "Admin",
				Token = GenerateToken() // Assign token at registration
			};

			_context.Users.Add(newUser);
			_context.SaveChanges();

			return RedirectToAction("Login" , "User");
		}





		// GET: User/Logout
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Login");
		}

		// Check if current user is Admin
		private bool IsAdmin()
		{
			return HttpContext.Session.GetString("Role") == "Admin";
		}

		// Hash Password using SHA256
		private string HashPassword(string password)
		{
			using (SHA256 sha256 = SHA256.Create())
			{
				byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
				return string.Concat(bytes.Select(b => b.ToString("x2")));
			}
		}

		// Generate a secure random token
		private string GenerateToken()
		{
			using (var rng = new RNGCryptoServiceProvider())
			{
				byte[] tokenBytes = new byte[32];
				rng.GetBytes(tokenBytes);
				return Convert.ToBase64String(tokenBytes);
			}
		}
		// Detination for user 

		public IActionResult Destinationview()
		{
			var destinations = _context.Destinations.ToList(); // Ensure this is not null
			return View(destinations);
		}

		// Dashboard for user

		[Authorize(Roles = "User")]
		public IActionResult Dashboard()
		{
		
			return View();
		}


		//Inquiries
		public IActionResult Inquiries()
		{
			 // Ensure this is not null
			return View();
		}



	   [HttpGet]
		public IActionResult SubmitInquiry()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SubmitInquiry(Inquiries inquiry)
		{
			if (ModelState.IsValid)
			{
				inquiry.InquiryDate = DateTime.Now;
				_context.Inquiries.Add(inquiry);
				_context.SaveChanges();

				TempData["SuccessMessage"] = "Your inquiry has been submitted successfully!";
				return RedirectToAction("Dashboard");
			}
			return View(inquiry);
		}

		public IActionResult AdminInquiries()
		{
			var inquiries = _context.Inquiries.OrderByDescending(i => i.InquiryDate).ToList();
			return View(inquiries);
		}



		// Feedback for user
		public IActionResult Feedback()
		{
			// Ensure this is not null
			return View();
		}

		[HttpGet]
		public IActionResult GetFeedbacks()
		{
			var feedback = _context.Feedback
				.Select(f => new {
					message = f.Message,  // Use "message" instead of "comment"
					submittedAt = f.SubmittedAt // Use "submittedAt" instead of "date"
				})
				.ToList();

			return Json(feedback);
		}

		[HttpPost]
		[ValidateAntiForgeryToken] // Helps prevent CSRF attacks
		public async Task<IActionResult> SubmitFeedback(Feedback feedback)
		{
			if (ModelState.IsValid)
			{
				feedback.SubmittedAt = DateTime.UtcNow; // Ensure timestamp is set
				_context.Feedback.Add(feedback);
				await _context.SaveChangesAsync();

				TempData["SuccessMessage"] = "Thank you for your feedback!";
				return RedirectToAction("Dashboard"); // Redirect to the destinations page
			}

			return View(feedback); // Return form with validation errors
		}


		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
