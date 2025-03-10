using MoviesApi.DTOs;
using MoviesApi.Entities;

namespace MoviesApi.Services
{
    /// <summary>
    /// Defines operations for managing users' favorite movies.
    /// </summary>
    public interface IFavoriteMovieService
    {
        /// <summary>
        /// Retrieves a paginated list of favorite movies for a given user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="title">An optional movie title to filter the results.</param>
        /// <param name="page">The page number for pagination.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A list of favorite movies.</returns>
        Task<List<FavoriteMovie>> GetFavoritesAsync(Guid userId, string? title, int page, int pageSize);

        /// <summary>
        /// Retrieves a specific favorite movie by its identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="favoriteId">The unique identifier of the favorite movie.</param>
        /// <returns>
        /// A tuple containing:
        /// - Found: Indicates whether the movie was found.
        /// - Message: A descriptive message.
        /// - Movie: The favorite movie if found, otherwise null.
        /// </returns>
        Task<(bool Found, string Message, FavoriteMovie? Movie)> GetFavoriteByIdAsync(Guid userId, Guid favoriteId);

        /// <summary>
        /// Adds a movie to the user's list of favorite movies.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="movie">The movie details to be added.</param>
        /// <returns>
        /// A tuple containing:
        /// - Success: Indicates whether the operation was successful.
        /// - Message: A descriptive message about the result.
        /// - Movie: The added favorite movie if successful, otherwise null.
        /// </returns>
        Task<(bool Success, string Message, FavoriteMovie? Movie)> AddFavoriteAsync(Guid userId, MovieDTO movie);

        /// <summary>
        /// Removes a movie from the user's list of favorite movies.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="favoriteId">The unique identifier of the favorite movie.</param>
        /// <returns>True if the movie was successfully removed; otherwise, false.</returns>
        Task<bool> RemoveFavoriteAsync(Guid userId, Guid favoriteId);
    }
}