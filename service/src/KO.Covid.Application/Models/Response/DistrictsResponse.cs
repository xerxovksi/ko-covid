namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class DistrictsResponse
    {
        [JsonProperty("districts")]
        public List<District> Districts { get; set; }

        [JsonProperty("ttl")]
        public int? TimeToLive { get; set; }
    }
}
