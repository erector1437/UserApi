using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserApi.Data;
using UserApi.Models;
using UserApi.Services;
using Xunit;

namespace UserTests
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly UserContext _context;

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<UserContext>()
                .UseInMemoryDatabase(databaseName: "UserDb")
                .Options;
            _context = new UserContext(options);
            _userService = new UserService(_context);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            // Arrange
            _context.Users.Add(new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(2000, 1, 1), PhoneNumber = "1234567890" });
            _context.Users.Add(new User { FirstName = "Jane", LastName = "Doe", Email = "jane.doe@example.com", DateOfBirth = new DateTime(1990, 1, 1), PhoneNumber = "0987654321" });
            await _context.SaveChangesAsync();

            // Act
            var users = await _userService.GetUsersAsync();

            // Assert
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            // Arrange
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(2000, 1, 1), PhoneNumber = "1234567890" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.Equal(user.Id, result.Id);
        }

        [Fact]
        public async Task CreateUserAsync_AddsUser()
        {
            // Arrange
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(2000, 1, 1), PhoneNumber = "1234567890" };

            // Act
            await _userService.CreateUserAsync(user);
            var users = await _userService.GetUsersAsync();

            // Assert
            Assert.Single(users);
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesUser()
        {
            // Arrange
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(2000, 1, 1), PhoneNumber = "1234567890" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            user.FirstName = "Jane";
            await _userService.UpdateUserAsync(user);
            var updatedUser = await _userService.GetUserByIdAsync(user.Id);

            // Assert
            Assert.Equal("Jane", updatedUser.FirstName);
        }

        [Fact]
        public async Task DeleteUserAsync_DeletesUser()
        {
            // Arrange
            var user = new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", DateOfBirth = new DateTime(2000, 1, 1), PhoneNumber = "1234567890" };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Act
            await _userService.DeleteUserAsync(user.Id);
            var users = await _userService.GetUsersAsync();

            // Assert
            Assert.Empty(users);
        }
    }
}
