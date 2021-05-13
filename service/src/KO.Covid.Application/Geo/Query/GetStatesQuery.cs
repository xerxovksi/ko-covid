namespace KO.Covid.Application.Geo
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetStatesQuery : IRequest<StatesResponse>
    {
        public string Mobile { get; set; }
    }
}
