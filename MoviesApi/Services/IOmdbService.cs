using MoviesApi.DTOs;

namespace MoviesApi.Services
{
    /// <summary>
    /// Provides functionality to interact with the OMDB API for retrieving movie information.
    /// </summary>
    public interface IOmdbService
    {
        /// <summary>
        /// Retrieves movie details from the OMDB API based on the given title.
        /// </summary>
        /// <param name="title">The title of the movie to search for.</param>
        /// <returns>
        /// A <see cref="MovieDTO"/> containing movie details if found; otherwise, null.
        /// </returns>
        Task<MovieDTO?> GetMovieByTitleAsync(string title);
    }
}
