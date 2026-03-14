using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Edinburgh_Internation_Students.Middleware;

public class LastActiveMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LastActiveMiddleware> _logger;

    public LastActiveMiddleware(RequestDelegate next, ILogger<LastActiveMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        // Update LastActive for authenticated users
        if (context.User.Identity?.IsAuthenticated == true)
        {
            try
            {
                var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                
                if (int.TryParse(userIdClaim, out int userId))
                {
                    var user = await dbContext.Users.FindAsync(userId);
                    
                    if (user != null)
                    {
                        user.LastActive = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error but don't fail the request
                _logger.LogError(ex, "Error updating LastActive for user");
            }
        }

        await _next(context);
    }
}
