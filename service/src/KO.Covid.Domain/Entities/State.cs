namespace KO.Covid.Domain.Entities
{
    using Newtonsoft.Json;

    public class State
    {
        [JsonProperty("state_id")]
        public int Id { get; set; }

        [JsonProperty("state_name")]
        public string Name { get; set; }

        [JsonProperty("state_name_l")]
        public string Name1 { get; set; }
    }
}
