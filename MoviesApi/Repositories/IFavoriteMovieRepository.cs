using MoviesApi.Entities;

namespace MoviesApi.Repositories
{
    /// <summary>
    /// Interface that defines operations for managing favorite movies.
    /// </summary>
    public interface IFavoriteMovieRepository
    {
        /// <summary>
        /// Retrieves a queryable collection of favorite movies for a specific user.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>An IQueryable collection of favorite movies.</returns>
        IQueryable<FavoriteMovie> GetFavoritesQueryByUser(Guid userId);

        /// <summary>
        /// Retrieves a specific favorite movie by its ID and user ID.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="movieId">Favorite movie's unique identifier.</param>
        /// <returns>The favorite movie entity if found; otherwise, null.</returns>
        Task<FavoriteMovie?> GetFavoriteByIdAsync(Guid userId, Guid movieId);

        /// <summary>
        /// Adds a new favorite movie to the database.
        /// </summary>
        /// <param name="movie">The favorite movie entity to add.</param>
        Task AddFavoriteAsync(FavoriteMovie movie);

        /// <summary>
        /// Removes a favorite movie from the database.
        /// </summary>
        /// <param name="movie">The favorite movie entity to remove.</param>
        Task RemoveFavoriteAsync(FavoriteMovie movie);

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        Task SaveChangesAsync();
    }
}
