using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    /// <summary>
    /// Represents the login credentials for user authentication.
    /// </summary>
    public class LoginDTO
    {
        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; } = string.Empty;
    }
}
