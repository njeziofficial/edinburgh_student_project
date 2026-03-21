using Edinburgh_Internation_Students.Data;
using Edinburgh_Internation_Students.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Edinburgh_Internation_Students.Middleware;

public class LastActiveMiddleware(RequestDelegate next, ILogger<LastActiveMiddleware> logger)
{
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
                logger.LogError(ex, "Error updating LastActive for user");
            }
        }

        await next(context);
    }
}
