using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using test4.Data;
using test4.Models;
using System;
using System.Threading.Tasks;

namespace test4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Accountant")]
    public class AccountantController : ControllerBase
    {
        private readonly LoanDbContext _context;

        public AccountantController(LoanDbContext context)
        {
            _context = context;
        }

        [HttpGet("All-Loans")]
        public async Task<IActionResult> GetUserLoans()
        {
            var loans = await _context.Loans.ToListAsync();
            return Ok(loans);
        }

        [Authorize(Roles = "Accountant")]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            var user = await _context.Users
                                      .Include(u => u.Loans)
                                      .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            user.HashedPassword = null;

            return Ok(user);
        }

        [HttpPut("loans/{loanId}/UpdateStatus")]
        public async Task<IActionResult> UpdateLoanStatus(int loanId, [FromBody] LoanStatus status)
        {
            try
            {
                var loan = await _context.Loans.FindAsync(loanId);

                if (loan == null)
                {
                    return NotFound("Loan not found.");
                }
                _context.Entry(loan).State = EntityState.Modified;
                loan.Status = status;
                await _context.SaveChangesAsync();

                return Ok(loan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }          
        [HttpPut("users/{userId}/block")]
        public async Task<IActionResult> BlockUser(int userId, [FromQuery] int blockDurationDays = 30)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
                }
                await _context.SaveChangesAsync();

                return Ok(new { message = "User blocked successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }
    }
}