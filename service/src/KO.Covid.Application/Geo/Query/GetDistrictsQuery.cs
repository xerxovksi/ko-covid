namespace KO.Covid.Application.Geo
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetDistrictsQuery : IRequest<DistrictsResponse>
    {
        public string StateName { get; set; }

        public string PublicToken { get; set; }
    }
}
