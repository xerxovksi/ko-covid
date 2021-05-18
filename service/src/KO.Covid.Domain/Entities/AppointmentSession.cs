namespace KO.Covid.Domain.Entities
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AppointmentSession
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        public string Date { get; set; }

        [JsonProperty("available_capacity")]
        public int? AvaialbleCapacity { get; set; }

        [JsonProperty("min_age_limit")]
        public int? MinimumAgeLimit { get; set; }

        public string Vaccine { get; set; }

        public List<string> Slots { get; set; }

        [JsonProperty("available_capacity_dose1")]
        public int? AvaialableCapacityDose1 { get; set; }

        [JsonProperty("available_capacity_dose2")]
        public int? AvaialableCapacityDose2 { get; set; }

        public AppointmentSession() =>
            this.Slots = new List<string>();
    }
}
