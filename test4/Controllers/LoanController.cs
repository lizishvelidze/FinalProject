using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using test4.Data;
using test4.DTOS;
using test4.Models;
using Microsoft.AspNetCore.Authorization;

namespace test4.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
    {
        private readonly LoanDbContext _context;

        public LoanController(LoanDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "User")]
        [HttpPost("request")]
        public async Task<IActionResult> CreateLoan([FromBody] LoanCreationDto loanDto)
        {
            try
            {
                var loan = new Loan
                {
                    Type = loanDto.Type,
                    Amount = loanDto.Amount,
                    Currency = loanDto.Currency,
                    Period = loanDto.Period,
                    UserId = loanDto.UserId
                };

                _context.Loans.Add(loan);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetLoanById), new { id = loan.Id }, loan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message, innerException = ex.InnerException?.Message });
            }
        }
       

        [Authorize(Roles = "User,Accountant")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLoanById(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound("Loan not found.");
            }

            return Ok(loan);
        }
        [Authorize(Roles = "User,Accountant")]
        [HttpPut("{id}/update")]
        public async Task<IActionResult> UpdateLoan(int id, [FromBody] LoanCreationDto loanDto)
        {
            var existingLoan = await _context.Loans.FindAsync(id);

            if (existingLoan == null)
            {
                return NotFound("Loan not found.");
            }

            try
            {
                _context.Entry(existingLoan).State = EntityState.Modified;

                existingLoan.Type = loanDto.Type;
                existingLoan.Amount = loanDto.Amount;
                existingLoan.Currency = loanDto.Currency;
                existingLoan.Period = loanDto.Period;

                await _context.SaveChangesAsync();
                return Ok(existingLoan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "User,Accountant")]
        [HttpDelete("{id}/delete")]
        public async Task<IActionResult> DeleteLoan(int id)
        {
            var loan = await _context.Loans.FindAsync(id);

            if (loan == null)
            {
                return NotFound("Loan not found.");
            }
            try
            {
                _context.Loans.Remove(loan);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}