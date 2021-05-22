namespace KO.Covid.Application.Authorization
{
    using FluentValidation;

    public class AddInternalTokenCommandValidator
        : AbstractValidator<AddInternalTokenCommand>
    {
        public AddInternalTokenCommandValidator()
        {
            RuleFor(request => request.InternalToken)
                .NotNull()
                .NotEmpty();
        }
    }
}
