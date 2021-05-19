namespace KO.Covid.Application.Authorization
{
    using FluentValidation;

    public class RegisterPincodeTokenCommandValidator
        : AbstractValidator<RegisterPincodeTokenCommand>
    {
        public RegisterPincodeTokenCommandValidator()
        {
            RuleFor(request => request.InternalPincodeToken)
                .NotNull()
                .NotEmpty();
        }
    }
}
