using System.Collections.Generic;
using System.Threading.Tasks;
using test4.Models;

namespace test4.Repositories
{
    public interface ILoanRepository
    {
        Task<Loan> GetByIdAsync(int id);
        Task<IEnumerable<Loan>> GetUserLoansAsync(int userId);
        Task<Loan> AddAsync(Loan loan);
        Task UpdateAsync(Loan loan);
        Task DeleteAsync(Loan loan);
        Task<IEnumerable<Loan>> GetAllLoansAsync();
    }
}