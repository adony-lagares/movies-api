using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services;
using System.Security.Claims;

namespace MoviesApi.Controllers;

/// <summary>
/// Controller responsible for managing users' favorite movies.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FavoritesController : ControllerBase
{
    private readonly IFavoriteMovieService _favoriteService;
    private readonly IOmdbService _omdbService;

    /// <summary>
    /// Initializes a new instance of <see cref="FavoritesController"/>.
    /// </summary>
    /// <param name="favoriteService">Service for managing favorite movies.</param>
    /// <param name="omdbService">Service for fetching movie data from OMDB API.</param>
    public FavoritesController(IFavoriteMovieService favoriteService, IOmdbService omdbService)
    {
        _favoriteService = favoriteService;
        _omdbService = omdbService;
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
    /// Retrieves the list of favorite movies for the authenticated user.
    /// </summary>
    /// <param name="title">Optional title filter.</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <returns>A list of favorite movies.</returns>
    [HttpGet]
    public async Task<IActionResult> GetFavorites([FromQuery] string? title, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var userId = GetUserId();
        var favorites = await _favoriteService.GetFavoritesAsync(userId, title, page, pageSize);
        return Ok(favorites);
    }

    /// <summary>
    /// Retrieves a favorite movie by its ID.
    /// </summary>
    /// <param name="id">The ID of the favorite movie.</param>
    /// <returns>The favorite movie details or an error message.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetFavoriteById(Guid id)
    {
        var userId = GetUserId();
        var (found, message, movie) = await _favoriteService.GetFavoriteByIdAsync(userId, id);

        if (!found)
            return NotFound(new { message });

        return Ok(new { message, movie });
    }

    /// <summary>
    /// Adds a movie to the user's favorite list.
    /// </summary>
    /// <param name="title">The title of the movie to be added.</param>
    /// <returns>The newly added favorite movie details or an error message.</returns>
    [HttpPost("{title}")]
    public async Task<IActionResult> AddFavorite(string title)
    {
        var userId = GetUserId();

        var movie = await _omdbService.GetMovieByTitleAsync(title);
        if (movie == null)
            return NotFound(new { message = "Movie not found" });

        var (success, message, favorite) = await _favoriteService.AddFavoriteAsync(userId, movie);
        if (!success)
            return BadRequest(new { message });

        return CreatedAtAction(nameof(GetFavoriteById), new { id = favorite!.Id }, new
        {
            message,
            movie
        });
    }

    /// <summary>
    /// Removes a movie from the user's favorite list.
    /// </summary>
    /// <param name="id">The ID of the favorite movie to be removed.</param>
    /// <returns>A success message or an error message.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> RemoveFavorite(Guid id)
    {
        var userId = GetUserId();
        bool removed = await _favoriteService.RemoveFavoriteAsync(userId, id);

        if (!removed)
            return NotFound(new { message = "Favorite not found" });

        return Ok(new { message = "Favorite movie removed successfully." });
    }
}
