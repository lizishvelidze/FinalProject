using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using test4.DTOS;
using test4.Models;
using test4.Repositories;
using BC = BCrypt.Net.BCrypt;

namespace test4.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogRepository _logRepository;

        public UserService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logRepository = logRepository;
        }

        public async Task<User> RegisterAsync(UserRegistrationDto registrationDto)
        {
            
            var existingUser = await _userRepository.GetByUsernameAsync(registrationDto.Username);
            if (existingUser != null)
            {
                await _logRepository.LogAsync("Warning", $"Registration attempt with existing username: {registrationDto.Username}");
                throw new ApplicationException("Username already exists");
            }

            var user = new User
            {
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
                Username = registrationDto.Username,
                Email = registrationDto.Email,
                Age = registrationDto.Age,
                MonthlyIncome = registrationDto.MonthlyIncome,
                HashedPassword = BC.HashPassword(registrationDto.Password),
                Role = "User" 
            };

            await _logRepository.LogAsync("Info", $"User registered: {user.Username}");
            return await _userRepository.AddAsync(user);
        }

        public async Task<string> LoginAsync(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByUsernameAsync(loginDto.Username);
            if (user == null || !BC.Verify(loginDto.Password, user.HashedPassword))
            {
                await _logRepository.LogAsync("Warning", $"Failed login attempt for username: {loginDto.Username}");
                throw new ApplicationException("Invalid username or password");
            }

            if (user.IsBlocked)
            {
                await _logRepository.LogAsync("Warning", $"Blocked user attempted login: {loginDto.Username}");
                throw new ApplicationException("User is blocked");
            }

            return GenerateJwtToken(user);
        }

        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                await _logRepository.LogAsync("Warning", $"User not found: {userId}");
                throw new ApplicationException("User not found");
            }
            return user;
        }

        public async Task BlockUserAsync(int userId, int blockDurationDays)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                await _logRepository.LogAsync("Warning", $"Attempt to block non-existent user: {userId}");
                throw new ApplicationException("User not found");
            }

            user.IsBlocked = true;
            await _userRepository.UpdateAsync(user);

            await _logRepository.LogAsync("Info", $"User blocked: {userId} for {blockDurationDays} days");
        }
    }
}
