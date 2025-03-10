using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using MoviesApi.DTOs;
using MoviesApi.Entities;
using MoviesApi.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Runtime.CompilerServices;
using System.Text;

[assembly: InternalsVisibleTo("MoviesApi.Tests")]

namespace MoviesApi.Services
{
    /// <summary>
    /// Service responsible for user management, authentication, and password security.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService"/> class.
        /// </summary>
        /// <param name="userRepository">User repository for database operations.</param>
        /// <param name="configuration">Application configuration settings.</param>
        /// <exception cref="ArgumentNullException">Thrown if any dependency is null.</exception>
        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="dto">User registration details.</param>
        /// <returns>The registered user if successful; otherwise, null.</returns>
        public async Task<User?> RegisterUserAsync(RegisterUserDTO dto)
        {
            if (await _userRepository.GetUserByEmailAsync(dto.Email) != null)
                return null; // Email already registered

            byte[] salt = GenerateSalt();

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = HashPassword(dto.Password, salt),
                Salt = Convert.ToBase64String(salt)
            };

            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        /// <summary>
        /// Authenticates a user and generates a JWT token if valid.
        /// </summary>
        /// <param name="dto">User login credentials.</param>
        /// <returns>A JWT token if authentication succeeds; otherwise, null.</returns>
        public async Task<string?> AuthenticateUserAsync(LoginDTO dto)
        {
            var user = await _userRepository.GetUserByEmailAsync(dto.Email);
            if (user == null || !VerifyPassword(dto.Password, user.PasswordHash, user.Salt))
                return null; // Invalid credentials

            return GenerateJwtToken(user);
        }

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>The user if found; otherwise, null.</returns>
        public async Task<User?> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        /// <summary>
        /// Updates a user's password after verifying the current password.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="dto">DTO containing the old and new password.</param>
        /// <returns>A tuple indicating success or failure with a descriptive message.</returns>
        public async Task<(bool Success, string Message)> UpdatePasswordAsync(Guid userId, UpdatePasswordDTO dto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                return (false, "User not found.");

            if (!VerifyPassword(dto.OldPassword, user.PasswordHash, user.Salt))
                return (false, "Incorrect current password.");

            user.PasswordHash = HashPassword(dto.NewPassword, Convert.FromBase64String(user.Salt));

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();

            return (true, "Password updated successfully.");
        }

        /// <summary>
        /// Generates a JWT token for authenticated users.
        /// </summary>
        /// <param name="user">The authenticated user.</param>
        /// <returns>A JWT token as a string.</returns>
        /// <exception cref="ArgumentNullException">Thrown if user is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if the JWT configuration is missing.</exception>
        private string GenerateJwtToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating JWT token.");

            var jwtSection = _configuration.GetSection("Jwt");
            string? keyString = jwtSection["Key"];
            string? issuer = jwtSection["Issuer"];
            string? audience = jwtSection["Audience"];

            if (string.IsNullOrEmpty(keyString))
                throw new InvalidOperationException("JWT key is missing in configuration.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Name)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Hashes a password using PBKDF2 with a given salt.
        /// </summary>
        /// <param name="password">The plain text password.</param>
        /// <param name="salt">The salt value for hashing.</param>
        /// <returns>The hashed password as a base64 string.</returns>
        internal string HashPassword(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32));
        }

        /// <summary>
        /// Verifies if a provided password matches the stored hashed password.
        /// </summary>
        /// <param name="password">Plain text password.</param>
        /// <param name="hashedPassword">Stored hashed password.</param>
        /// <param name="storedSalt">Stored salt value.</param>
        /// <returns>True if the password is valid; otherwise, false.</returns>
        private bool VerifyPassword(string password, string hashedPassword, string storedSalt)
        {
            byte[] salt = Convert.FromBase64String(storedSalt);
            return HashPassword(password, salt) == hashedPassword;
        }

        /// <summary>
        /// Generates a cryptographically secure random salt.
        /// </summary>
        /// <returns>A byte array containing the salt.</returns>
        private byte[] GenerateSalt()
        {
            byte[] salt = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
