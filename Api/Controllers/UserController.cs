using Application.DTO;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;
    
    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    private int GetCurrentUserId()
    {
        if (HttpContext.Items["UserId"] is int userId) 
            return userId;
        
        throw new UnauthorizedAccessException("User not authenticated!");
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        // Authentication via Middleware
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });
        
        // Getting userId from middleware
        int userId = GetCurrentUserId();

        var profile = await _userService.GetUserProfile(userId);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile(UpdateProfileDTO updateDTO)
    {
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });

        int userId = GetCurrentUserId();
        
        var updatedProfile = await _userService.UpdateUserProfile(userId, updateDTO);
        return Ok(updatedProfile);
        
    }

    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
    {
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });
        
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        
        int userId = GetCurrentUserId();
        
        await _userService.ChangePassword(userId, changePasswordDTO);
        return Ok();
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });

        int userId = GetCurrentUserId();

        await _userService.DeleteAccount(userId);
        
        Response.Cookies.Delete("sessionId");
        return Ok();
    }
}