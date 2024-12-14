using System.Collections.Generic;
using System.Threading.Tasks;

namespace test4.Repositories
{
    public interface ILogRepository
    {
        Task LogAsync(string level, string message, string exception = null);
    }
}