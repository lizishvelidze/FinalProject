using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace test4.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MaxLength(50)]
        public string Username { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MonthlyIncome { get; set; }
        public bool IsBlocked { get; set; } = false;
        [Required]
        public string HashedPassword { get; set; }
        public string Role { get; set; }
        public ICollection<Loan> Loans { get; set; }
    }
}
