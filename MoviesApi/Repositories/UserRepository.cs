using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Entities;

namespace MoviesApi.Repositories
{
    /// <summary>
    /// Repository responsible for managing user-related database operations.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserRepository"/> class.
        /// </summary>
        /// <param name="context">Database context.</param>
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <inheritdoc />
        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <inheritdoc />
        public async Task<bool> UpdateUserAsync(User updatedUser)
        {
            var existingUser = await _context.Users.FindAsync(updatedUser.Id);
            if (existingUser == null) return false;

            // Only update fields if they have changed
            if (!string.IsNullOrEmpty(updatedUser.PasswordHash) && existingUser.PasswordHash != updatedUser.PasswordHash)
            {
                existingUser.PasswordHash = updatedUser.PasswordHash;
            }

            existingUser.Name = updatedUser.Name;
            existingUser.Email = updatedUser.Email;

            _context.Users.Update(existingUser);
            await SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> RemoveUserAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await SaveChangesAsync();
            return true;
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
