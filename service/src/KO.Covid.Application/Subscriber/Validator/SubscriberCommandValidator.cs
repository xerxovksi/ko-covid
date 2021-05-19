namespace KO.Covid.Application.Subscriber
{
    using FluentValidation;
    using KO.Covid.Domain;

    public class SubscriberCommandValidator : AbstractValidator<SubscriberCommand>
    {
        private const int Limit = 3;

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

            When(
                request => !request.Subscriber.Districts.IsNullOrEmpty(),
                () => RuleFor(
                    request => request.Subscriber.Districts.Count)
                .LessThanOrEqualTo(Limit)
                .WithMessage(
                    $"Subscriber.Districts.Count should be less than or equal to {Limit}."));

            When(
                request => !request.Subscriber.Pincodes.IsNullOrEmpty(),
                () => RuleFor(
                    request => request.Subscriber.Pincodes.Count)
                .LessThanOrEqualTo(Limit)
                .WithMessage(
                    $"Subscriber.Pincodes.Count should be less than or equal to {Limit}."));
        }
    }
}
