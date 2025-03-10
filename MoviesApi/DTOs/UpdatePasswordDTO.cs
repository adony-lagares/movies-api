using System.ComponentModel.DataAnnotations;

namespace MoviesApi.DTOs
{
    /// <summary>
    /// Represents the data transfer object for updating a user's password.
    /// </summary>
    public class UpdatePasswordDTO
    {
        /// <summary>
        /// Gets or sets the user's current password.
        /// </summary>
        [Required(ErrorMessage = "Current password is required.")]
        public string OldPassword { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the new password. Must be at least 6 characters long.
        /// </summary>
        [Required(ErrorMessage = "New password is required.")]
        [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
