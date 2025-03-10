using MoviesApi.DTOs;
using MoviesApi.Entities;

namespace MoviesApi.Services
{
    /// <summary>
    /// Defines operations related to user management, including authentication, registration, 
    /// password updates, and retrieval of user data.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="dto">User registration data.</param>
        /// <returns>
        /// A <see cref="User"/> object if registration is successful; otherwise, null if the email is already in use.
        /// </returns>
        Task<User?> RegisterUserAsync(RegisterUserDTO dto);

        /// <summary>
        /// Authenticates a user and generates a JWT token if credentials are valid.
        /// </summary>
        /// <param name="dto">User login data.</param>
        /// <returns>
        /// A JWT token as a string if authentication is successful; otherwise, null.
        /// </returns>
        Task<string?> AuthenticateUserAsync(LoginDTO dto);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">Unique identifier of the user.</param>
        /// <returns>
        /// A <see cref="User"/> object if found; otherwise, null.
        /// </returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Updates a user's password after validating the current password.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="dto">Password update data.</param>
        /// <returns>
        /// A tuple containing a boolean indicating success and a message describing the result.
        /// </returns>
        Task<(bool Success, string Message)> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO dto);
    }
}
