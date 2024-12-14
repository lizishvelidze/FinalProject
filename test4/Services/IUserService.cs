using System.Threading.Tasks;
using test4.DTOS;
using test4.Models;

namespace test4.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(UserRegistrationDto registrationDto);
        Task<string> LoginAsync(UserLoginDto loginDto);
        Task<User> GetUserByIdAsync(int userId);
        Task BlockUserAsync(int userId, int blockDurationDays);
    }
}
