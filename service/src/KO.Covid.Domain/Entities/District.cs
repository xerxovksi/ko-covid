namespace KO.Covid.Domain.Entities
{
    using Newtonsoft.Json;

    public class District
    {
        [JsonProperty("state_id")]
        public int StateId { get; set; }

        [JsonProperty("district_id")]
        public int Id { get; set; }

        [JsonProperty("district_name")]
        public string Name { get; set; }

        [JsonProperty("district_name_l")]
        public string Name1 { get; set; }
    }
}
