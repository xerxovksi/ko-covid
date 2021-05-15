namespace KO.Covid.Application.Contracts
{
    using MediatR;
    using System.Threading.Tasks;

    public interface IEventMediator<TResult>
    {
        Task<TResult> SendAsync<TRequest>(IRequest<TRequest> request);
    }
}