using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using test4.Controllers;
using test4.Data;
using test4.DTOS;
using test4.Models;
using Xunit;

namespace test4.tests.ControllerTests
{
    public class UserControllerTests
    {
        private readonly LoanDbContext _context;
        private readonly ILogger<UserController> _logger;

        public UserControllerTests()
        {
            var options = new DbContextOptionsBuilder<LoanDbContext>()
                .UseInMemoryDatabase("UserTestDb")
                .Options;

            _context = new LoanDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<UserController>();

            _context.Users.AddRange(
                new User
                {
                    Id = 1,
                    Username = "testuser",
                    Email = "testuser@test.com",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("password123"),
                    Role = "User",
                    IsBlocked = false
                },
                new User
                {
                    Id = 2,
                    Username = "accountant1",
                    Email = "accountant@test.com",
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword("accountant123"),
                    Role = "Accountant",
                    IsBlocked = false
                }
            );

            _context.SaveChanges();
        }

        [Fact]
        public async Task Register_ShouldCreateNewUser()
        {
            // Arrange
            var controller = new UserController(_context, _logger);
            var registrationDto = new UserRegistrationDto
            {
                FirstName = "John",
                LastName = "Doe",
                Username = "newuser",
                Email = "newuser@test.com",
                Password = "password123",
                Age = 30,
                MonthlyIncome = 4000
            };

            // Act
            var result = await controller.Register(registrationDto) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            var createdUser = result.Value as User;
            Assert.NotNull(createdUser);
            Assert.Equal("newuser", createdUser.Username);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenUsernameExists()
        {
            // Arrange
            var controller = new UserController(_context, _logger);
            var registrationDto = new UserRegistrationDto
            {
                Username = "testuser",
                Email = "duplicate@test.com",
                Password = "password123"
            };

            // Act
            var result = await controller.Register(registrationDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);          
        }

        [Fact]
        public async Task Login_ShouldReturnToken_WhenCredentialsAreValid()
        {
            // Arrange
            var controller = new UserController(_context, _logger);
            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "password123"
            };

            // Act
            var result = await controller.Login(loginDto) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);           
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var controller = new UserController(_context, _logger);
            var loginDto = new UserLoginDto
            {
                Username = "testuser",
                Password = "wrongpassword"
            };

            // Act
            var result = await controller.Login(loginDto) as UnauthorizedObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(401, result.StatusCode);
        }

        [Fact]
        public async Task RegisterAccountant_ShouldCreateNewAccountant()
        {
            // Arrange
            var controller = new UserController(_context, _logger);
            var registrationDto = new UserRegistrationDto
            {
                FirstName = "Jane",
                LastName = "Smith",
                Username = "accountant2",
                Email = "accountant2@test.com",
                Password = "password123",
                Age = 35,
                MonthlyIncome = 5000
            };

            // Act
            var result = await controller.RegisterAccountant(registrationDto) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(201, result.StatusCode);
            var createdAccountant = result.Value as User;
            Assert.NotNull(createdAccountant);
            Assert.Equal("accountant2", createdAccountant.Username);
            Assert.Equal("Accountant", createdAccountant.Role);
        }
    }
}