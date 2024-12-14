using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using test4.Data;
using test4.Models;
using test4.Repositories;
using test4.Services;
using Xunit;

public class AccountantServiceTests
{
    private readonly LoanDbContext _context;
    private readonly AccountantService _accountantService;

    public AccountantServiceTests()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase("AccountantTestDb")
            .Options;

        _context = new LoanDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _accountantService = new AccountantService(
            new LoanRepository(_context),
            new LogRepository(_context)
        );

        _context.Loans.AddRange(
            new Loan { Id = 3, UserId = 3, Amount = 1000, Currency = "USD", Status = LoanStatus.InProgress },
            new Loan { Id = 4, UserId = 4, Amount = 5000, Currency = "EUR", Status = LoanStatus.Approved }
        );
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllLoansAsync_ShouldReturnAllLoans()
    {
        // Act
        var loans = await _accountantService.GetAllLoansAsync();

        // Assert
        Assert.NotNull(loans);
        Assert.Equal(2, loans.Count());  
    }

    [Fact]
    public async Task UpdateLoanStatusAsync_ShouldUpdateLoanStatus_WhenLoanExists()
    {
        // Arrange
        var loanId = 3;  
        var newStatus = LoanStatus.Approved;  

        // Act
        var updatedLoan = await _accountantService.UpdateLoanStatusAsync(loanId, newStatus);

        // Assert
        Assert.NotNull(updatedLoan);
        Assert.Equal(newStatus, updatedLoan.Status); 

        var loanFromDb = await _context.Loans.FindAsync(loanId);
        Assert.NotNull(loanFromDb);
        Assert.Equal(newStatus, loanFromDb.Status);  
    }

    [Fact]
    public async Task UpdateLoanStatusAsync_ShouldThrowException_WhenLoanNotFound()
    {
        // Arrange
        var loanId = 999;  
        var newStatus = LoanStatus.Rejected;

        // Act 
        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            _accountantService.UpdateLoanStatusAsync(loanId, newStatus));

        // Assert
        Assert.Equal("Loan not found", exception.Message);
    }

    [Fact]
    public async Task DeleteLoanAsync_ShouldDeleteLoan_WhenLoanExists()
    {
        // Arrange
        var loanId = 4; 

        // Act
        var result = await _accountantService.DeleteLoanAsync(loanId);

        // Assert
        Assert.True(result);  
        var loanFromDb = await _context.Loans.FindAsync(loanId);
        Assert.Null(loanFromDb);
    }

    [Fact]
    public async Task DeleteLoanAsync_ShouldThrowException_WhenLoanNotFound()
    {
        // Arrange
        var loanId = 999;  

        // Act 
        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            _accountantService.DeleteLoanAsync(loanId));

        // Assert
        Assert.Equal("Loan not found", exception.Message);
    }
}
