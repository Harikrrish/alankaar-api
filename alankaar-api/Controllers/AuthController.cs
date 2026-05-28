using alankaar_api.Data;
using alankaar_api.DTOs;
using alankaar_api.Models;
using alankaar_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace alankaar_api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AlankaarDbContext context, IPasswordHashService passwordHasher) : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var normalizedEmail = AuthText.NormalizeEmail(request.Email);
        var user = await context.Users
            .SingleOrDefaultAsync(user => user.NormalizedEmail == normalizedEmail);

        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        user.LastLoginAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        return Ok(ToResponse(user, "Login successful."));
    }

    private static AuthResponse ToResponse(User user, string message)
    {
        return new AuthResponse
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role,
            Message = message
        };
    }
}
