using System.Collections.Generic;
using System.Threading.Tasks;
using test4.Models;

namespace test4.Services
{
    public interface IAccountantService
    {
        Task<IEnumerable<Loan>> GetAllLoansAsync();
        Task<Loan> UpdateLoanStatusAsync(int loanId, LoanStatus status);
        Task<bool> DeleteLoanAsync(int loanId);
    }
}
