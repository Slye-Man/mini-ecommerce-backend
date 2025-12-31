using Domain;

namespace Api.Middleware;

public class AuthenticationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var sessionId = context.Request.Cookies["sessionId"];

        if (!string.IsNullOrEmpty(sessionId))
        {
            // TODO: Validate session from browser session storage
            
            // Storing session ID for controllers usage
            context.Items["sessionId"] = sessionId;
            context.Items["isAuthenticated"] = true;
        }
        
        await next(context);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}