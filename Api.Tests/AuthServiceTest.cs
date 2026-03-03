using Api;
using Application.DTO;
using Domain;
using Domain.Carts;
using Domain.Users;
using FluentAssertions;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Api.Tests;

public class AuthServiceTest : IDisposable
{
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly Mock<ISessionService> _sessionServiceMock;
    private readonly ApplicationDbContext _context;
    private readonly AuthService _authService;

    public AuthServiceTest()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _loggerMock = new Mock<ILogger<AuthService>>();
        _sessionServiceMock = new Mock<ISessionService>();
        _authService = new AuthService(_context, _loggerMock.Object, _sessionServiceMock.Object);
    }

    #region RegisterUser Tests

    [Fact]
    public async Task RegisterUser_WithValidData_ShouldCreateUserAndCart()
    {
        // Arrange
        var registerDTO = new RegisterRequestDTO
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "Password123!",
            ConfirmPassword = "Password123!"
        };

        // Act
        var result = await _authService.RegisterUser(registerDTO);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(registerDTO.Email);
        result.UserName.Should().Be(registerDTO.UserName);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
        user.Should().NotBeNull();
        user.Password.Should().NotBe(registerDTO.Password);

        var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == user.UserId);
        cart.Should().NotBeNull();
    }
    
    [Fact]
    public async Task RegisterUser_WithExistingEmail_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = new User
        {
            Email = "existing@example.com",
            UserName = "existinguser",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDTO = new RegisterRequestDTO
        {
            Email = "existing@example.com",
            UserName = "newuser",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.RegisterUser(registerDTO));
    }

    [Fact]
    public async Task RegisterUser_WithExistingUsername_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var existingUser = new User
        {
            Email = "user@example.com",
            UserName = "existinguser",
            Password = BCrypt.Net.BCrypt.HashPassword("Password123"),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(existingUser);
        await _context.SaveChangesAsync();

        var registerDTO = new RegisterRequestDTO
        {
            Email = "newemail@example.com",
            UserName = "existinguser",
            Password = "Password123",
            ConfirmPassword = "Password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _authService.RegisterUser(registerDTO));
    }

    [Fact]
    public async Task RegisterUser_ShouldHashPassword()
    {
        // Arrange
        var registerDTO = new RegisterRequestDTO
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "PlainTextPassword",
            ConfirmPassword = "PlainTextPassword"
        };

        // Act
        await _authService.RegisterUser(registerDTO);

        // Assert
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == registerDTO.Email);
        user.Should().NotBeNull();
        user!.Password.Should().NotBe("PlainTextPassword");
        BCrypt.Net.BCrypt.Verify("PlainTextPassword", user.Password).Should().BeTrue();
    }

    #endregion

    #region LoginUser Tests

    [Fact]
    public async Task LoginUser_WithValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Password123");
        var user = new User
        {
            Email = "login@example.com",
            UserName = "loginuser",
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDTO = new LoginRequestDTO
        {
            Username = "loginuser",
            Password = "Password123"
        };

        var expectedSessionId = "test-session-id";
        _sessionServiceMock
            .Setup(s => s.CreateSession(It.IsAny<int>()))
            .ReturnsAsync(expectedSessionId);

        // Act
        var result = await _authService.LoginUser(loginDTO);

        // Assert
        result.Should().NotBeNull();
        result.Email.Should().Be(user.Email);
        result.UserName.Should().Be(user.UserName);
        result.UserId.Should().Be(user.UserId);
        result.SessionId.Should().Be(expectedSessionId);

        _sessionServiceMock.Verify(s => s.CreateSession(user.UserId), Times.Once);
}

    [Fact]
    public async Task LoginUser_WithInvalidUsername_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var loginDTO = new LoginRequestDTO
        {
            Username = "nonexistent",
            Password = "Password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginUser(loginDTO));
}

    [Fact]
    public async Task LoginUser_WithInvalidPassword_ShouldThrowUnauthorizedAccessException()
    {
        // Arrange
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var loginDTO = new LoginRequestDTO
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            async () => await _authService.LoginUser(loginDTO));
    }

    #endregion

    #region ValidateSession Tests

    [Fact]
    public async Task ValidateSession_WithValidSession_ShouldReturnTrue()
    {
        // Arrange
        var sessionId = "valid-session-id";
        _sessionServiceMock
            .Setup(s => s.ValidateSession(sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.ValidateSession(sessionId);

        // Assert
        result.Should().BeTrue();
        _sessionServiceMock.Verify(s => s.ValidateSession(sessionId), Times.Once);
}

    [Fact]
    public async Task ValidateSession_WithInvalidSession_ShouldReturnFalse()
    {
        // Arrange
        var sessionId = "invalid-session-id";
        _sessionServiceMock
            .Setup(s => s.ValidateSession(sessionId))
            .ReturnsAsync(false);

        // Act
        var result = await _authService.ValidateSession(sessionId);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region GetUserBySession Tests

    [Fact]
    public async Task GetUserBySession_WithValidSession_ShouldReturnUser()
    {
        // Arrange
        var user = new User
        {
            Email = "test@example.com",
            UserName = "testuser",
            Password = "hashedpassword",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var sessionId = "valid-session";
        _sessionServiceMock
            .Setup(s => s.GetUserIdFromSession(sessionId))
            .ReturnsAsync(user.UserId);

        // Act
        var result = await _authService.GetUserBySession(sessionId);

        // Assert
        result.Should().NotBeNull();
        result.UserId.Should().Be(user.UserId);
        result.UserName.Should().Be(user.UserName);
}

    [Fact]
    public async Task GetUserBySession_WithInvalidSession_ShouldReturnNull()
    {
        // Arrange
        var sessionId = "invalid-session";
        _sessionServiceMock
            .Setup(s => s.GetUserIdFromSession(sessionId))
            .ReturnsAsync((int?)null);

        // Act
        var result = await _authService.GetUserBySession(sessionId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region LogoutUser Tests

    [Fact]
    public async Task LogoutUser_WithValidSession_ShouldReturnTrue()
    {
        // Arrange
        var sessionId = "valid-session";
        _sessionServiceMock
            .Setup(s => s.DeleteSession(sessionId))
            .ReturnsAsync(true);

        // Act
        var result = await _authService.LogoutUser(sessionId);

        // Assert
        result.Should().BeTrue();
        _sessionServiceMock.Verify(s => s.DeleteSession(sessionId), Times.Once);
}

    [Fact]
    public async Task LogoutUser_WithEmptySessionId_ShouldReturnFalse()
    {
        // Arrange
        var sessionId = string.Empty;

        // Act
        var result = await _authService.LogoutUser(sessionId);

        // Assert
        result.Should().BeFalse();
        _sessionServiceMock.Verify(s => s.DeleteSession(It.IsAny<string>()), Times.Never);
    }

    #endregion

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
}
