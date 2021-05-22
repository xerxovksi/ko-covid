namespace KO.Covid.Application.Authorization
{
    using FluentValidation;

    public class AddPublicTokenCommandValidator
        : AbstractValidator<AddPublicTokenCommand>
    {
        public AddPublicTokenCommandValidator()
        {
            RuleFor(request => request.PublicToken)
                .NotNull()
                .NotEmpty();
        }
    }
}
