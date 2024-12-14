using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;
using test4.Data;
using test4.DTOS;
using test4.Models;
using System;
using System.Linq;
using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Logging;
using test4.Services;

namespace test4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly LoanDbContext _context;
        private readonly ILogger<UserController> _logger;
        
        public UserController(LoanDbContext context, ILogger<UserController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto registrationDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registrationDto.Username))
            {
                return BadRequest(new { message = "Username already exists" });
            }

            if (await _context.Users.AnyAsync(u => u.Email == registrationDto.Email))
            {
                return BadRequest(new { message = "Email already exists" });
            }

            try
            {
                var user = new User
                {
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Username = registrationDto.Username,
                    Email = registrationDto.Email,
                    Age = registrationDto.Age,
                    MonthlyIncome = registrationDto.MonthlyIncome,
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                    Role = "User",
                    IsBlocked = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                user.HashedPassword = null;

                return CreatedAtAction(nameof(GetUserProfile), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserLoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                if (user.IsBlocked)
                {
                    return Unauthorized(new { message = "User account is blocked" });
                }

                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.HashedPassword))
                {
                    return Unauthorized(new { message = "Invalid username or password" });
                }
                var stringToken = GenerateJwtToken(user);
                return Ok(new
                {
                    token = stringToken
                });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "User,Accountant")]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            try
            {
                var user = await _context.Users
                    .Include(u => u.Loans)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }

                user.HashedPassword = null;

                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }     
    
        [HttpPost("register-accountant")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAccountant([FromBody] UserRegistrationDto registrationDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registrationDto.Username && u.Role == "Accountant"))
            {
                return BadRequest(new { message = "username already exists" });
            }

            try
            {
                var accountant = new User
                {
                    FirstName = registrationDto.FirstName,
                    LastName = registrationDto.LastName,
                    Username = registrationDto.Username,
                    Email = registrationDto.Email,
                    Age = registrationDto.Age,
                    MonthlyIncome = registrationDto.MonthlyIncome,
                    HashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationDto.Password),
                    Role = "Accountant", 
                    IsBlocked = false
                };

                _context.Users.Add(accountant);
                await _context.SaveChangesAsync();

                accountant.HashedPassword = null; 

                return CreatedAtAction(nameof(GetUserProfile), new { id = accountant.Id }, accountant);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            
            var key = Encoding.ASCII.GetBytes("YourVeryLongAndSecureSecretKeyHere!@#$%^&*()_+");

           
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)

                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature) 
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            return tokenString;
        }
    }
}