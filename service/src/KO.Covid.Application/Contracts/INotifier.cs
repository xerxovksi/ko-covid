namespace KO.Covid.Application.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface INotifier
    {
        Task SendAsync(
            string subject,
            List<string> recepients,
            string message);
    }
}
