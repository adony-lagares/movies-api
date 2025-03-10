using Moq;
using MoviesApi.Services;
using MoviesApi.Repositories;
using MoviesApi.Entities;
using MoviesApi.DTOs;
using Microsoft.Extensions.Configuration;
using Xunit;
using System;
using System.Threading.Tasks;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly UserService _userService;

    /// <summary>
    /// Initializes a new instance of <see cref="UserServiceTests"/>.
    /// Mocks the dependencies and sets up the configuration for JWT authentication.
    /// </summary>
    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configMock = new Mock<IConfiguration>();

        // Mock the JWT configuration to avoid null references during token generation
        var jwtSectionMock = new Mock<IConfigurationSection>();
        jwtSectionMock.Setup(x => x["Key"]).Returns("ThisIsAVeryStrongAndLongSecretKey123456!");
        jwtSectionMock.Setup(x => x["Issuer"]).Returns("MoviesApi");
        jwtSectionMock.Setup(x => x["Audience"]).Returns("MoviesApiUsers");

        _configMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSectionMock.Object);

        _userService = new UserService(_userRepositoryMock.Object, _configMock.Object);
    }

    /// <summary>
    /// Tests if a new user is successfully registered.
    /// </summary>
    [Fact]
    public async Task RegisterUserAsync_ShouldReturnUser_WhenSuccessful()
    {
        // Arrange
        var dto = new RegisterUserDTO { Name = "Test User", Email = "test@example.com", Password = "123456" };
        var user = new User { Id = Guid.NewGuid(), Email = dto.Email, Name = dto.Name };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null); // Simulates that the email is not already registered

        _userRepositoryMock.Setup(repo => repo.AddUserAsync(It.IsAny<User>()))
            .Returns(Task.CompletedTask);

        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.RegisterUserAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        Assert.Equal(dto.Name, result.Name);
    }

    /// <summary>
    /// Tests if authentication returns a JWT token when credentials are valid.
    /// </summary>
    /// <param name="dto">The login credentials of the user.</param>
    [Fact]
    public async Task AuthenticateUserAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
        var dto = new LoginDTO { Email = "test@example.com", Password = "123456" };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            Name = "Test User",
            PasswordHash = _userService.HashPassword(dto.Password, Convert.FromBase64String("testSalt")),
            Salt = "testSalt"
        };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync(user);

        // Act
        var token = await _userService.AuthenticateUserAsync(dto);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    /// <summary>
    /// Tests if authentication fails and returns null when credentials are incorrect.
    /// </summary>
    /// <param name="dto">The incorrect login credentials.</param>
    [Fact]
    public async Task AuthenticateUserAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
    {
        // Arrange
        var dto = new LoginDTO { Email = "wrong@example.com", Password = "wrongpassword" };

        _userRepositoryMock.Setup(repo => repo.GetUserByEmailAsync(dto.Email))
            .ReturnsAsync((User?)null); // Simulates that the user does not exist

        // Act
        var token = await _userService.AuthenticateUserAsync(dto);

        // Assert
        Assert.Null(token);
    }

    /// <summary>
    /// Tests if a user's password is successfully updated when the current password is correct.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="dto">The DTO containing the old and new passwords.</param>
    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnSuccess_WhenCurrentPasswordIsCorrect()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = _userService.HashPassword("oldpassword", Convert.FromBase64String("testSalt")),
            Salt = "testSalt"
        };

        var updatePasswordDto = new UpdatePasswordDTO { OldPassword = "oldpassword", NewPassword = "newpassword" };

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user))
            .ReturnsAsync(true);

        _userRepositoryMock.Setup(repo => repo.SaveChangesAsync())
            .Returns(Task.CompletedTask);

        // Act
        var result = await _userService.UpdatePasswordAsync(userId, updatePasswordDto);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("Password updated successfully.", result.Message);
    }

    /// <summary>
    /// Tests if an error is returned when attempting to update the password with an incorrect current password.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="dto">The DTO containing the incorrect current password and new password.</param>
    [Fact]
    public async Task UpdatePasswordAsync_ShouldReturnFailure_WhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            Email = "test@example.com",
            Name = "Test User",
            PasswordHash = _userService.HashPassword("correctpassword", Convert.FromBase64String("testSalt")),
            Salt = "testSalt"
        };

        var updatePasswordDto = new UpdatePasswordDTO { OldPassword = "wrongpassword", NewPassword = "newpassword" };

        _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UpdatePasswordAsync(userId, updatePasswordDto);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Incorrect current password.", result.Message);
    }
}
