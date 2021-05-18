namespace KO.Covid.Application.Authorization
{
    using FluentValidation;

    public class RegisterDistrictTokenCommandValidator
        : AbstractValidator<RegisterDistrictTokenCommand>
    {
        public RegisterDistrictTokenCommandValidator()
        {
            RuleFor(request => request.InternalDistrictToken)
                .NotNull()
                .NotEmpty()
                .WithMessage("InternalDistrictToken should not be null or empty.");
        }
    }
}
