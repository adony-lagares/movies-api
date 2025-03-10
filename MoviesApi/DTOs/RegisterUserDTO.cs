using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    /// <summary>
    /// Represents the data transfer object for registering a new user.
    /// </summary>
    public class RegisterUserDTO
    {
        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name must not exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

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
