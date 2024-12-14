using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using test4.Data;
using test4.DTOS;
using test4.Models;
using test4.Repositories;
using test4.Services;
using Xunit;

public class LoanServiceTests
{
    private readonly LoanDbContext _context;
    private readonly LoanService _loanService;

    public LoanServiceTests()
    {
        var options = new DbContextOptionsBuilder<LoanDbContext>()
            .UseInMemoryDatabase("LoanTestDb")
            .Options;

        _context = new LoanDbContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _loanService = new LoanService(
            new LoanRepository(_context), 
            new UserRepository(_context),   
            new LogRepository(_context)   
        );
        
        _context.Users.Add(new User { Id = 7, Username = "testuser", IsBlocked = false });
        _context.SaveChanges();
    }

    [Fact]
    public async Task CreateLoanAsync_ShouldCreateLoan_WhenUserIsValid()
    {
        // Arrange
        var userId = 7; 
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.QuickLoan,  
            Amount = 2000,              
            Currency = "USD",           
            Period = 12                 
        };

        // Act
        var result = await _loanService.CreateLoanAsync(userId, loanDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(2000, result.Amount);
        Assert.Equal(LoanStatus.InProgress, result.Status);  
        Assert.Equal(LoanType.QuickLoan, result.Type);       
        Assert.Equal("USD", result.Currency);               

        var loanFromDb = await _context.Loans.FindAsync(result.Id);
        Assert.NotNull(loanFromDb);
        Assert.Equal(userId, loanFromDb.UserId);
        Assert.Equal(2000, loanFromDb.Amount);
        Assert.Equal(LoanType.QuickLoan, loanFromDb.Type);  
    }

    [Fact]
    public async Task UpdateLoanAsync_ShouldUpdateLoan_WhenLoanIsValid()
    {
        // Arrange
        var userId = 7;
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.Installment,
            Amount = 3000,
            Currency = "EUR",
            Period = 24
        };
        
        var createdLoan = await _loanService.CreateLoanAsync(userId, loanDto);

        var updatedLoanDto = new LoanCreationDto
        {
            Type = LoanType.AutoLoan, 
            Amount = 5000,            
            Currency = "USD",         
            Period = 36
        };

        // Act
        var updatedLoan = await _loanService.UpdateLoanAsync(userId, createdLoan.Id, updatedLoanDto);

        // Assert
        Assert.NotNull(updatedLoan);
        Assert.Equal(5000, updatedLoan.Amount);
        Assert.Equal(LoanType.AutoLoan, updatedLoan.Type);  
        Assert.Equal("USD", updatedLoan.Currency);          
        Assert.Equal(36, updatedLoan.Period);              

        var loanFromDb = await _context.Loans.FindAsync(updatedLoan.Id);
        Assert.NotNull(loanFromDb);
        Assert.Equal(5000, loanFromDb.Amount);
        Assert.Equal(LoanType.AutoLoan, loanFromDb.Type);
    }
    [Fact]
    public async Task UpdateLoanAsync_ShouldThrowException_WhenLoanNotFound()
    {
        // Arrange
        var userId = 7;
        var nonExistentLoanId = 888;  
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.Installment,
            Amount = 3000,
            Currency = "EUR",
            Period = 24
        };
        // Act 
        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            _loanService.UpdateLoanAsync(userId, nonExistentLoanId, loanDto));

        // Assert
        Assert.Equal("Loan not found or unauthorized", exception.Message);
    }
    [Fact]
    public async Task DeleteLoanAsync_ShouldDeleteLoan_WhenLoanIsInProgress()
    {
        // Arrange
        var userId = 7;
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.QuickLoan,
            Amount = 1000,
            Currency = "USD",
            Period = 12
        };

        var createdLoan = await _loanService.CreateLoanAsync(userId, loanDto);

        // Act
        var result = await _loanService.DeleteLoanAsync(userId, createdLoan.Id);

        // Assert
        Assert.True(result);  
        var loanFromDb = await _context.Loans.FindAsync(createdLoan.Id);
        Assert.Null(loanFromDb); 
    }
    [Fact]
    public async Task DeleteLoanAsync_ShouldThrowException_WhenLoanNotFound()
    {
        // Arrange
        var userId = 7;
        var nonExistentLoanId = 999; 

        // Act 
        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            _loanService.DeleteLoanAsync(userId, nonExistentLoanId));
        // Assert
        Assert.Equal("Loan not found or unauthorized", exception.Message);
    }

    [Fact]
    public async Task CreateLoanAsync_ShouldThrowException_WhenUserIsBlocked()
    {
        // Arrange
        var userId = 7;
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.QuickLoan,
            Amount = 2000,
            Currency = "USD",
            Period = 12
        };
       
        var user = await _context.Users.FindAsync(userId);
        user.IsBlocked = true;
        await _context.SaveChangesAsync();

        // Act 
        var exception = await Assert.ThrowsAsync<ApplicationException>(() =>
            _loanService.CreateLoanAsync(userId, loanDto));

        // Assert
        Assert.Equal("User is blocked from creating loans", exception.Message);
    }
    [Fact]
    public async Task GetUserLoansAsync_ShouldReturnUserLoans_WhenLoansExist()
    {
        // Arrange
        var userId = 7;
        var loanDto = new LoanCreationDto
        {
            Type = LoanType.QuickLoan,
            Amount = 2000,
            Currency = "USD",
            Period = 12
        };
       
        await _loanService.CreateLoanAsync(userId, loanDto);

        // Act
        var loans = await _loanService.GetUserLoansAsync(userId);

        // Assert
        Assert.NotNull(loans);
        Assert.Single(loans);  
        Assert.Equal(userId, loans.First().UserId);  
    }


}
