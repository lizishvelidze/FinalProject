using System.ComponentModel.DataAnnotations;

namespace test4.DTOS
{
    public class UserRegistrationDto
    {
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
        [Range(18, 120)]
        public int Age { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal MonthlyIncome { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }
}