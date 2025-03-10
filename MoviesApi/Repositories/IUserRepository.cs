using MoviesApi.Entities;

namespace MoviesApi.Repositories
{
    /// <summary>
    /// Interface that defines operations for managing user data.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retrieves a user by email.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        Task<User?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>The user entity if found; otherwise, null.</returns>
        Task<User?> GetUserByIdAsync(Guid userId);

        /// <summary>
        /// Checks whether an email is already in use.
        /// </summary>
        /// <param name="email">User's email.</param>
        /// <returns>True if the email exists; otherwise, false.</returns>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Adds a new user to the database.
        /// </summary>
        /// <param name="user">User entity to add.</param>
        Task AddUserAsync(User user);

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="user">Updated user entity.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        Task<bool> UpdateUserAsync(User user);

        /// <summary>
        /// Removes a user from the database.
        /// </summary>
        /// <param name="userId">User's unique identifier.</param>
        /// <returns>True if the user was removed; otherwise, false.</returns>
        Task<bool> RemoveUserAsync(Guid userId);

        /// <summary>
        /// Saves changes to the database asynchronously.
        /// </summary>
        Task SaveChangesAsync();
    }
}
