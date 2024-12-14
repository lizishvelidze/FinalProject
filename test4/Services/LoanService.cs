using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using test4.DTOS;
using test4.Models;
using test4.Repositories;

namespace test4.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public LoanService(
            ILoanRepository loanRepository,
            IUserRepository userRepository,
            ILogRepository logRepository)
        {
            _loanRepository = loanRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public async Task<Loan> CreateLoanAsync(int userId, LoanCreationDto loanDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                await _logRepository.LogAsync("Warning", $"Loan creation attempt for non-existent user: {userId}");
                throw new ApplicationException("User not found");
            }

            if (user.IsBlocked)
            {
                await _logRepository.LogAsync("Warning", $"Blocked user attempted loan creation: {userId}");
                throw new ApplicationException("User is blocked from creating loans");
            }

            var loan = new Loan
            {
                UserId = userId,
                Type = loanDto.Type,
                Amount = loanDto.Amount,
                Currency = loanDto.Currency,
                Period = loanDto.Period,
                Status = LoanStatus.InProgress
            };

            await _logRepository.LogAsync("Info", $"Loan created for user: {userId}");
            return await _loanRepository.AddAsync(loan);
        }

        public async Task<Loan> UpdateLoanAsync(int userId, int loanId, LoanCreationDto loanDto)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null || loan.UserId != userId)
            {
                await _logRepository.LogAsync("Warning", $"Unauthorized loan update attempt: User {userId}, Loan {loanId}");
                throw new ApplicationException("Loan not found or unauthorized");
            }

            if (loan.Status != LoanStatus.InProgress)
            {
                await _logRepository.LogAsync("Warning", $"Attempt to update non-in-progress loan: {loanId}");
                throw new ApplicationException("Only in-progress loans can be updated");
            }

            loan.Type = loanDto.Type;
            loan.Amount = loanDto.Amount;
            loan.Currency = loanDto.Currency;
            loan.Period = loanDto.Period;

            await _logRepository.LogAsync("Info", $"Loan updated: {loanId}");
            await _loanRepository.UpdateAsync(loan);
            return loan;
        }

        public async Task<bool> DeleteLoanAsync(int userId, int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null || loan.UserId != userId)
            {
                await _logRepository.LogAsync("Warning", $"Unauthorized loan deletion attempt: User {userId}, Loan {loanId}");
                throw new ApplicationException("Loan not found or unauthorized");
            }

            if (loan.Status != LoanStatus.InProgress)
            {
                await _logRepository.LogAsync("Warning", $"Attempt to delete non-in-progress loan: {loanId}");
                throw new ApplicationException("Only in-progress loans can be deleted");
            }

            await _loanRepository.DeleteAsync(loan);
            await _logRepository.LogAsync("Info", $"Loan deleted: {loanId}");
            return true;
        }

        public async Task<IEnumerable<Loan>> GetUserLoansAsync(int userId)
        {
            return await _loanRepository.GetUserLoansAsync(userId);
        }
    }
}
