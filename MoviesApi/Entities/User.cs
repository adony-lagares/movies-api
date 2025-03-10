using System.ComponentModel.DataAnnotations;

namespace MoviesApi.Entities
{
    /// <summary>
    /// Represents a user entity in the application.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the hashed password of the user.
        /// </summary>
        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the salt used for password hashing.
        /// </summary>
        public string Salt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of favorite movies associated with the user.
        /// </summary>
        public ICollection<FavoriteMovie> FavoriteMovies { get; set; } = new List<FavoriteMovie>();
    }
}
