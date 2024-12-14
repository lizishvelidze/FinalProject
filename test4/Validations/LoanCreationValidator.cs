using FluentValidation;
using test4.DTOS;

namespace test4.Validations
{
    public class LoanCreationValidator : AbstractValidator<LoanCreationDto>
    {
        public LoanCreationValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Loan amount must be positive")
                .LessThan(1000000).WithMessage("Loan amount is too high");

            RuleFor(x => x.Currency)
                .NotEmpty().WithMessage("Currency is required")
                .Length(3).WithMessage("Currency must be 3 characters");

            RuleFor(x => x.Period)
                .GreaterThan(0).WithMessage("Loan period must be positive")
                .LessThan(60).WithMessage("Loan period cannot exceed 60 months");
        }
    }
}