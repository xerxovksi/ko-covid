namespace KO.Covid.Application.Contracts
{
    using MediatR;
    using System;
    using System.Threading.Tasks;

    public interface IEventMediator<TResult>
    {
        Task<TResult> SendAsync<TResponse>(
            IRequest<TResponse> request,
            Func<TResponse, string> successLogMessage = null,
            Func<TResponse, object[]> successLogParameters = null);
    }
}