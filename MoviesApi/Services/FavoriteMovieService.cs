using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MoviesApi.Services
{
    /// <summary>
    /// Service responsible for managing user's favorite movies.
    /// </summary>
    public class FavoriteMovieService : IFavoriteMovieService
    {
        private readonly IFavoriteMovieRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteMovieService"/> class.
        /// </summary>
        /// <param name="repository">The repository for favorite movies.</param>
        public FavoriteMovieService(IFavoriteMovieRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task<List<FavoriteMovie>> GetFavoritesAsync(Guid userId, string? title, int page, int pageSize)
        {
            var query = _repository.GetFavoritesQueryByUser(userId);

            if (!string.IsNullOrEmpty(title))
            {
                query = query.Where(f => f.Title.Contains(title));
            }

            return await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        /// <inheritdoc />
        public async Task<(bool Found, string Message, FavoriteMovie? Movie)> GetFavoriteByIdAsync(Guid userId, Guid favoriteId)
        {
            var favorite = await _repository.GetFavoriteByIdAsync(userId, favoriteId);

            if (favorite == null)
                return (false, "This movie is not in your favorites list.", null);

            return (true, "Movie found in favorites.", favorite);
        }

        /// <inheritdoc />
        public async Task<(bool Success, string Message, FavoriteMovie? Movie)> AddFavoriteAsync(Guid userId, MovieDTO movie)
        {
            if (string.IsNullOrEmpty(movie.ImdbID))
                return (false, "Invalid movie ID.", null);

            var existingFavorite = await _repository
                .GetFavoritesQueryByUser(userId)
                .FirstOrDefaultAsync(f => f.ImdbId == movie.ImdbID);

            if (existingFavorite != null)
                return (false, "This movie is already in your favorites.", existingFavorite);

            var favorite = new FavoriteMovie
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ImdbId = movie.ImdbID,
                Title = movie.Title,
                Year = movie.Year,
                Director = movie.Director,
                Poster = movie.Poster
            };

            await _repository.AddFavoriteAsync(favorite);
            await _repository.SaveChangesAsync();
            return (true, "Movie added to favorites successfully.", favorite);
        }

        /// <inheritdoc />
        public async Task<bool> RemoveFavoriteAsync(Guid userId, Guid favoriteId)
        {
            var favorite = await _repository.GetFavoriteByIdAsync(userId, favoriteId);
            if (favorite == null) return false;

            await _repository.RemoveFavoriteAsync(favorite);
            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
