using FluentValidation;
namespace OperationService.Api.Validators
{
    public class TextValidator : AbstractValidator<string>
    {
        public TextValidator()
        {
            RuleFor(x => x)
                .NotNull()
                .NotEmpty().WithMessage("Text cannot be empty")
                .MaximumLength(500).WithMessage("Text cannot exceed 500 characters");
        }
    }
}
