using System.ComponentModel.DataAnnotations;
using test4.Models;

namespace test4.DTOS
{
    public class LoanCreationDto
    {
        [Required]
        public LoanType Type { get; set; }
        [Required]
        [Range(100, 1000000)]
        public decimal Amount { get; set; }
        [Required]
        public string Currency { get; set; }
        [Required]
        [Range(1, 60)]
        public int Period { get; set; }
        public int UserId { get; set; }
    }
}
