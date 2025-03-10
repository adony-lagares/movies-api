using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.DTOs;
using MoviesApi.Services;
using System.Security.Claims;

namespace MoviesApi.Controllers;

/// <summary>
/// Controller responsible for user authentication and profile management.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    /// <summary>
    /// Initializes a new instance of <see cref="UsersController"/>.
    /// </summary>
    /// <param name="userService">The user service.</param>
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves the user ID from the authenticated JWT token.
    /// </summary>
    /// <returns>The user ID.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when the user token is invalid.</exception>
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim) : throw new UnauthorizedAccessException("Invalid user token.");
    }

    /// <summary>
    /// Registers a new user.
    /// </summary>
    /// <param name="dto">User registration data.</param>
    /// <returns>The created user details or an error message.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDTO dto)
    {
        try
        {
            var user = await _userService.RegisterUserAsync(dto);
            if (user == null)
                return BadRequest(new { message = "Email already in use." });

            return CreatedAtAction(nameof(GetProfile), new { id = user.Id }, new
            {
                message = "User created successfully.",
                user = new { user.Id, user.Name, user.Email }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Authenticates a user and returns a JWT token.
    /// </summary>
    /// <param name="dto">User login data.</param>
    /// <returns>The JWT token or an error message.</returns>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        try
        {
            var token = await _userService.AuthenticateUserAsync(dto);
            if (token == null)
                return Unauthorized(new { message = "Invalid email or password." });

            return Ok(new { token });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Retrieves the authenticated user's profile information.
    /// </summary>
    /// <returns>The user's profile details.</returns>
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new { user.Id, user.Name, user.Email });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred.", error = ex.Message });
        }
    }

    /// <summary>
    /// Allows the authenticated user to update their password.
    /// </summary>
    /// <param name="dto">Password update data.</param>
    /// <returns>A success message or an error message.</returns>
    [HttpPut("update-password")]
    [Authorize]
    public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordDTO dto)
    {
        try
        {
            var userId = GetUserId();
            var result = await _userService.UpdatePasswordAsync(userId, dto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = "Password updated successfully." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An internal error occurred.", error = ex.Message });
        }
    }
}
