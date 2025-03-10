using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Try.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
});

// Configure authentication with Cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/User/Login"; // Redirect if not authenticated
		options.AccessDeniedPath = "/User/AccessDenied"; // Redirect if unauthorized
		options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set expiration time
		options.SlidingExpiration = true;
	});

// Configure database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MVC services
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure middleware pipeline
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Ensure static assets (CSS, JS, images) are served

app.UseRouting();

app.UseSession(); // Enable session middleware

app.UseAuthentication(); // Enable authentication (before authorization)
app.UseAuthorization();  // Enable role-based authorization

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
