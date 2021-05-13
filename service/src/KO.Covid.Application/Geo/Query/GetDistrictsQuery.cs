namespace KO.Covid.Application.Geo
{
    using KO.Covid.Application.Models;
    using MediatR;

    public class GetDistrictsQuery : IRequest<DistrictsResponse>
    {
        public string Mobile { get; set; }

        public string StateName { get; set; }
    }
}
