using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    /// <summary>
    /// Represents a movie data transfer object retrieved from the OMDB API.
    /// </summary>
    public class MovieDTO
    {
        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the release year of the movie.
        /// </summary>
        [Required(ErrorMessage = "Year is required.")]
        public string Year { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the movie's director.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the movie's poster image.
        /// </summary>
        public string Poster { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the IMDb identifier for the movie.
        /// </summary>
        [Required(ErrorMessage = "IMDb ID is required.")]
        public string ImdbID { get; set; } = string.Empty;
    }
}
