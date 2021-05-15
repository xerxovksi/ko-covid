namespace KO.Covid.Application.Models
{
    using KO.Covid.Domain.Entities;
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class StatesResponse
    {
        public List<State> States { get; set; }

        [JsonProperty("ttl")]
        public int? TimeToLive { get; set; }

        public StatesResponse() =>
            this.States = new List<State>();
    }
}
