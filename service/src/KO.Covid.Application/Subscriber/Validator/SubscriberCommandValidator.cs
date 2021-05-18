namespace KO.Covid.Application.Subscriber
{
    using FluentValidation;
    using KO.Covid.Domain;

    public class SubscriberCommandValidator : AbstractValidator<SubscriberCommand>
    {
        public SubscriberCommandValidator()
        {
            RuleFor(request => request.Subscriber)
                .NotNull()
                .WithMessage("Subscriber should not be null.");

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Mobile)
                .NotNull()
                .NotEmpty()
                .Matches(@"^\d{10}$")
                .WithMessage("Subscriber.Mobile is invalid."));

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Email)
                .NotNull()
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Subscriber.Email is invalid."));

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => request.Subscriber.Age)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Subscriber.Age is invalid."));

            When(
                request => request.Subscriber != null,
                () => RuleFor(
                    request => !request.Subscriber.Districts.IsNullOrEmpty()
                    || !request.Subscriber.Pincodes.IsNullOrEmpty())
                .NotEqual(false)
                .WithMessage(
                    "Either Subscriber.Districts or Subscriber.Pincodes should be non-empty."));
        }
    }
}
