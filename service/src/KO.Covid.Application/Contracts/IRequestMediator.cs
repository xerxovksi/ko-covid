namespace KO.Covid.Application.Contracts
{
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading.Tasks;

    public interface IRequestMediator
    {
        Task<IActionResult> SendAsync<T>(
            IRequest<T> request,
            Action validate = null,
            Func<T, string> successLogMessage = null,
            Func<T, object[]> successLogParameters = null);
    }
}
