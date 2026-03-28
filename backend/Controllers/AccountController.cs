using System.Security.Claims;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly SignInManager<AppUser> _signInManager;
    private readonly UserManager<AppUser> _userManager;

    public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        if (User.Identity is not { IsAuthenticated: true })
            return Ok(null);

        AppUser? user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Ok(null);

        return Ok(new { user.Email, isEmailConfirmed = user.EmailConfirmed });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await _signInManager.SignOutAsync();
        return NoContent();
    }
}
