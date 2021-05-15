namespace KO.Covid.Infrastructure.Subscriber
{
    public class SubscriberContainer
    {
        public string Id { get; }

        public SubscriberContainer(string containerId) =>
            this.Id = containerId;
    }
}
