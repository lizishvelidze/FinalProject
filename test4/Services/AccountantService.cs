using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using test4.Models;
using test4.Repositories;

namespace test4.Services
{
    public class AccountantService : IAccountantService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILogRepository _logRepository;

        public AccountantService(
            ILoanRepository loanRepository,
            ILogRepository logRepository)
        {
            _loanRepository = loanRepository;
            _logRepository = logRepository;
        }

        public async Task<IEnumerable<Loan>> GetAllLoansAsync()
        {
            await _logRepository.LogAsync("Info", "Accountant retrieved all loans");
            return await _loanRepository.GetAllLoansAsync();
        }

        public async Task<Loan> UpdateLoanStatusAsync(int loanId, LoanStatus status)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                await _logRepository.LogAsync("Warning", $"Attempt to update status of non-existent loan: {loanId}");
                throw new ApplicationException("Loan not found");
            }

            loan.Status = status;
            await _loanRepository.UpdateAsync(loan);

            await _logRepository.LogAsync("Info", $"Loan status updated: {loanId} to {status}");
            return loan;
        }

        public async Task<bool> DeleteLoanAsync(int loanId)
        {
            var loan = await _loanRepository.GetByIdAsync(loanId);
            if (loan == null)
            {
                await _logRepository.LogAsync("Warning", $"Attempt to delete non-existent loan: {loanId}");
                throw new ApplicationException("Loan not found");
            }

            await _loanRepository.DeleteAsync(loan);
            await _logRepository.LogAsync("Info", $"Loan deleted by accountant: {loanId}");
            return true;
        }
    }
}