using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

public class RoleMiddleware
{
	private readonly RequestDelegate _next;

	public RoleMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		// Check if the user is authenticated
		if (context.User.Identity.IsAuthenticated)
		{
			// Retrieve the role from session or claims
			var userRole = context.Session.GetString("Role"); // If using session
			
			// Alternatively, get role from claims (if using Identity)
			// var userRole = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

			// Redirect user if they don't have a required role (modify as needed)
			if (string.IsNullOrEmpty(userRole))
			{
				context.Response.Redirect("/User/AccessDenied");
				return;
			}
		}

		// Continue request pipeline
		await _next(context);
	}
}
