using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi.Repositories
{
    /// <summary>
    /// Repository for managing favorite movies in the database.
    /// Implements IFavoriteMovieRepository.
    /// </summary>
    public class FavoriteMovieRepository : IFavoriteMovieRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteMovieRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public FavoriteMovieRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a queryable collection of favorite movies for a specific user.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>An IQueryable collection of favorite movies.</returns>
        public IQueryable<FavoriteMovie> GetFavoritesQueryByUser(Guid userId)
        {
            return _context.FavoriteMovies
                .Where(f => f.UserId == userId)
                .AsQueryable();
        }

        /// <summary>
        /// Retrieves a specific favorite movie by its ID and user ID.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <param name="movieId">Favorite movie's unique identifier.</param>
        /// <returns>The favorite movie entity if found; otherwise, null.</returns>
        public async Task<FavoriteMovie?> GetFavoriteByIdAsync(Guid userId, Guid movieId)
        {
            return await _context.FavoriteMovies
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == movieId && f.UserId == userId);
        }

        /// <summary>
        /// Adds a new favorite movie to the database.
        /// </summary>
        /// <param name="movie">The favorite movie entity to add.</param>
        public async Task AddFavoriteAsync(FavoriteMovie movie)
        {
            await _context.FavoriteMovies.AddAsync(movie);
        }

        /// <summary>
        /// Removes a favorite movie from the database.
        /// </summary>
        /// <param name="movie">The favorite movie entity to remove.</param>
        public Task RemoveFavoriteAsync(FavoriteMovie movie)
        {
            _context.FavoriteMovies.Remove(movie);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
