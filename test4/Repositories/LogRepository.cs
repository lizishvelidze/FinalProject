using System.Threading.Tasks;
using System;
using test4.Data;
using Microsoft.Extensions.Logging;

namespace test4.Repositories
{
    public class Log
    {
        public int Id { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class LogRepository : ILogRepository
    {
        private readonly LoanDbContext _context;
    

        public LogRepository(LoanDbContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string level, string message, string exception = null)
        {
            var logs = new Log
            {
                Level = level,
                Message = message,
                Exception = exception,
                Timestamp = DateTime.UtcNow
            };

            await _context.Logs.AddAsync(logs);
            await _context.SaveChangesAsync();
        }
    }
}

