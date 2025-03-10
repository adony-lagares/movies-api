using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MoviesApi.Entities
{
    /// <summary>
    /// Represents a favorite movie entity associated with a user.
    /// </summary>
    public class FavoriteMovie
    {
        /// <summary>
        /// Gets or sets the unique identifier for the favorite movie entry.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the IMDb ID of the movie.
        /// </summary>
        [Required]
        public string ImdbId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title of the movie.
        /// </summary>
        [Required]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the release year of the movie.
        /// </summary>
        [Required]
        public string Year { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the movie director.
        /// </summary>
        public string Director { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the movie poster image.
        /// </summary>
        public string Poster { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the user who favorited the movie.
        /// </summary>
        [Required]
        [ForeignKey("User")]
        public Guid UserId { get; set; }

        /// <summary>
        /// Navigation property for the user who favorited the movie.
        /// </summary>
        public User? User { get; set; }
    }
}
