namespace KO.Covid.Application.Authorization
{
    using FluentValidation;

    public class AddActiveUserCommandValidator
        : AbstractValidator<AddActiveUserCommand>
    {
        public AddActiveUserCommandValidator()
        {
            RuleFor(request => request.Mobile)
                .NotNull()
                .NotEmpty()
                .Matches(@"^\d{10}$")
                .WithMessage("Should be a valid 10 digit mobile number.");
        }
    }
}
