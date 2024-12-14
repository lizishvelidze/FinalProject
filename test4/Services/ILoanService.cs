using System.Collections.Generic;
using System.Threading.Tasks;
using test4.DTOS;
using test4.Models;

namespace test4.Services
{
    public interface ILoanService
    {
        Task<Loan> CreateLoanAsync(int userId, LoanCreationDto loanDto);
        Task<Loan> UpdateLoanAsync(int userId, int loanId, LoanCreationDto loanDto);
        Task<bool> DeleteLoanAsync(int userId, int loanId);
        Task<IEnumerable<Loan>> GetUserLoansAsync(int userId);
    }
}
