using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using test4.Controllers;
using test4.Data;
using test4.DTOS;
using test4.Models;
using Xunit;

namespace test4.tests.ControllerTests
{
    public class LoanControllerTests
    {
        private LoanDbContext _context;

        public LoanControllerTests()
        {
            var options = new DbContextOptionsBuilder<LoanDbContext>()
                .UseInMemoryDatabase("LoanTestDb")
                .Options;

            _context = new LoanDbContext(options);

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            _context.Loans.AddRange(
                new Loan { Id = 1, Type = (LoanType)1, Amount = 10000, Currency = "USD", Period = 12 },
                new Loan { Id = 2, Type = (LoanType)2, Amount = 50000, Currency = "USD", Period = 24 }
            );
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateLoan_ShouldReturnCreatedLoan()
        {
            // Arrange
            var controller = new LoanController(_context);
            var loanDto = new LoanCreationDto
            {
                Type = (LoanType)2,
                Amount = 20000,
                Currency = "USD",
                Period = 36
            };

            // Act
            var result = await controller.CreateLoan(loanDto) as CreatedAtActionResult;
            var createdLoan = result.Value as Loan;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(createdLoan);
            Assert.Equal(20000, createdLoan.Amount);
            Assert.Equal((LoanType)2, createdLoan.Type);
        }
        [Fact]
        public async Task GetLoanById_ShouldReturnLoan()
        {
            // Arrange
            var controller = new LoanController(_context);

            // Act
            var result = await controller.GetLoanById(1) as OkObjectResult;
            var loan = result.Value as Loan;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, loan.Id);
            Assert.Equal(10000, loan.Amount);
            Assert.Equal((LoanType)1, loan.Type);
        }

        [Fact]
        public async Task GetLoanById_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new LoanController(_context);

            // Act
            var result = await controller.GetLoanById(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Loan not found.", result.Value);
        }
        [Fact]
        public async Task UpdateLoan_ShouldReturnUpdatedLoan()
        {
            // Arrange
            var controller = new LoanController(_context);
            var loanDto = new LoanCreationDto
            {
                Type = (LoanType)1,
                Amount = 60000,
                Currency = "USD",
                Period = 48
            };

            // Act
            var result = await controller.UpdateLoan(1, loanDto) as OkObjectResult;
            var updatedLoan = result.Value as Loan;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(60000, updatedLoan.Amount);
            Assert.Equal((LoanType)1, updatedLoan.Type);
            Assert.Equal(48, updatedLoan.Period);
        }

        [Fact]
        public async Task UpdateLoan_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new LoanController(_context);
            var loanDto = new LoanCreationDto
            {
                Type = (LoanType)1,
                Amount = 60000,
                Currency = "USD",
                Period = 48
            };

            // Act
            var result = await controller.UpdateLoan(999, loanDto) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Loan not found.", result.Value);
        }

        [Fact]
        public async Task DeleteLoan_ShouldReturnNoContent()
        {
            // Arrange
            var controller = new LoanController(_context);

            // Act
            var result = await controller.DeleteLoan(1) as NoContentResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public async Task DeleteLoan_ShouldReturnNotFound()
        {
            // Arrange
            var controller = new LoanController(_context);

            // Act
            var result = await controller.DeleteLoan(999) as NotFoundObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Loan not found.", result.Value);
        }
    }
}


