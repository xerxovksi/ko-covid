namespace KO.Covid.Domain.Entities
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class Appointment
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("center_id")]
        public string CenterId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }

        [JsonProperty("district_name")]
        public string DistrictName { get; set; }

        [JsonProperty("block_name")]
        public string BlockName { get; set; }

        public int? Pincode { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        [JsonProperty("fee_type")]
        public string FeeType { get; set; }

        public string Date { get; set; }

        public string Fee { get; set; }

        [JsonProperty("min_age_limit")]
        public int? MinimumAgeLimit { get; set; }

        public string Vaccine { get; set; }

        public List<string> Slots { get; set; }
    }
}
