using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using test4.Controllers;
using test4.Data;
using test4.Models;
using Xunit;

namespace test4.tests.ControllerTests
{
    public class AccountantControllerTests
    {
        private readonly LoanDbContext _context;

        public AccountantControllerTests()
        {
            var options = new DbContextOptionsBuilder<LoanDbContext>()
                .UseInMemoryDatabase("AccountantTestDb")
                .Options;

            _context = new LoanDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Loans.AddRange(
                new Loan { Id = 1, Amount = 1000, Status = LoanStatus.InProgress },
                new Loan { Id = 2, Amount = 2000, Status = LoanStatus.Approved }
            );

            _context.Users.Add(new User { Id = 1, Username = "testuser", IsBlocked = false });
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetUserLoans_ShouldReturnAllLoans()
        {
            // Arrange
            var controller = new AccountantController(_context);

            // Act
            var result = await controller.GetUserLoans() as OkObjectResult;
            var loans = result.Value as List<Loan>;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(loans);
            Assert.Equal(2, loans.Count);
        }

        [Fact]
        public async Task UpdateLoanStatus_ShouldReturnUpdatedLoan()
        {
            // Arrange
            var controller = new AccountantController(_context);
            var updatedStatus = LoanStatus.Approved;

            // Act
            var result = await controller.UpdateLoanStatus(1, updatedStatus) as OkObjectResult;
            var loan = result.Value as Loan;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.NotNull(loan);
            Assert.Equal(updatedStatus, loan.Status);
         
            var loanFromDb = await _context.Loans.FindAsync(1);
            Assert.Equal(updatedStatus, loanFromDb.Status);
        }

        [Fact]
        public async Task UpdateLoanStatus_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new AccountantController(_context);

            // Act
            var result = await controller.UpdateLoanStatus(999, LoanStatus.Rejected) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("Loan not found.", result.Value);
        }

        [Fact]
        public async Task BlockUser_ShouldBlockUserSuccessfully()
        {
            // Arrange
            var controller = new AccountantController(_context);
            int userIdToBlock = 1;
            
            var result = await controller.BlockUser(userIdToBlock, 30) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var response = result.Value as dynamic;
            Assert.NotNull(response);
            Assert.Equal("User blocked successfully", response.message);

            var blockedUser = await _context.Users.FindAsync(userIdToBlock);
            Assert.NotNull(blockedUser);
            Assert.True(blockedUser.IsBlocked); 
        }

        [Fact]
        public async Task BlockUser_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new AccountantController(_context);

            // Act
            var result = await controller.BlockUser(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
            Assert.Equal("User not found.", result.Value);
        }
    }
}
