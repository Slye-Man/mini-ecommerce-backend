using Domain;
using Infrastructure.Services;

namespace Api.Middleware;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, IAuthService authService)
    {
        var sessionId = context.Request.Cookies["sessionId"] ?? context.Request.Cookies["SessionId"];

        if (!string.IsNullOrEmpty(sessionId))
        {
            // Validate the session
            var isValid = await authService.ValidateSession(sessionId);
            
            if (isValid)
            {
                // Get user from session
                var user = await authService.GetUserBySession(sessionId);
                
                if (user != null)
                {
                    // Store session and user information for controllers
                    context.Items["sessionId"] = sessionId;
                    context.Items["isAuthenticated"] = true;
                    context.Items["User"] = user;
                    context.Items["UserId"] = user.UserId;
                }
                else
                {
                    // Session valid but user not found - clean up
                    _logger.LogWarning("Session {SessionId} is valid but user not found", sessionId);
                    context.Response.Cookies.Delete("SessionId");
                    context.Response.Cookies.Delete("sessionId");
                }
            }
            else
            {
                // Session invalid or expired - remove cookie
                _logger.LogDebug("Invalid or expired session: {SessionId}", sessionId);
                context.Response.Cookies.Delete("SessionId");
                context.Response.Cookies.Delete("sessionId");
            }
        }

        await _next(context);
    }
}

public static class AuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<AuthenticationMiddleware>();
    }
}