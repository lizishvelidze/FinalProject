using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace test4.Models
{
    public enum LoanType
    {
        QuickLoan,
        AutoLoan,
        Installment
    }

    public enum LoanStatus
    {
        InProgress,
        Approved,
        Rejected
    }
    public class Loan
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public LoanType Type { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }
        [Required]
        [MaxLength(10)]
        public string Currency { get; set; }
        [Required]
        public int Period { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.InProgress;
     
        public int UserId { get; set; }
        public User User { get; set; }
    }
}