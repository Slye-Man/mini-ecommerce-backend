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

    [HttpGet("profile")]
    public async Task<IActionResult> GetUserProfile()
    {
        // Authentication via Middleware
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });
        
        // TODO: Get userId from browser session
        // Using mock userID
        var sessionId = HttpContext.Items["SessionId"] as string;
        
        // Mock UserID
        int userId = 1;

        var profile = await _userService.GetUserProfile(userId);
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateUserProfile(UpdateProfileDTO updateDTO)
    {
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });

        int userId = 1;
        
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
        
        int userId = 1;
        
        await _userService.ChangePassword(userId, changePasswordDTO);
        return Ok();
    }

    [HttpDelete("account")]
    public async Task<IActionResult> DeleteAccount()
    {
        if (HttpContext.Items["isAuthenticated"] as bool? != true)
            return Unauthorized(new { message = "Not Authenticated" });

        int userId = 1;

        await _userService.DeleteAccount(userId);
        
        Response.Cookies.Delete("sessionId");
        return Ok();
    }
}