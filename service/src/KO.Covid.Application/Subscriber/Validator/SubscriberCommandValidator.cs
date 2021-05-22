namespace KO.Covid.Application.Subscriber
{
    using FluentValidation;

    public class SubscriberCommandValidator : AbstractValidator<SubscriberCommand>
    {
        private const int MinimumLimit = 1;
        private const int MaximumLimit = 3;

        public SubscriberCommandValidator()
        {
            RuleFor(request => request.Subscriber)
                .NotNull();

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Name)
                .NotNull()
                .NotEmpty());

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Mobile)
                .NotNull()
                .NotEmpty()
                .Matches(@"^\d{10}$")
                .WithMessage("Should be a valid 10 digit mobile number."));

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Should be a valid email address."));

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Age)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Should be greater than 0."));

            When(
                request => request.Subscriber.Districts != null,
                () => RuleFor(
                    request => request.Subscriber.Districts.Count)
                .GreaterThanOrEqualTo(MinimumLimit)
                .LessThanOrEqualTo(MaximumLimit)
                .WithMessage($"Should be between {MinimumLimit} and {MaximumLimit}."));
        }
    }
}
