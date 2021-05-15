namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class DistrictsResponse
    {
        public List<District> Districts { get; set; }

        [JsonProperty("ttl")]
        public int? TimeToLive { get; set; }

        public DistrictsResponse() =>
            this.Districts = new List<District>();
    }
}
