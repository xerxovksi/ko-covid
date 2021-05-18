namespace KO.Covid.Domain.Entities
{
    using Newtonsoft.Json;
    using System.Collections.Generic;

    public class AppointmentCenter
    {
        [JsonProperty("center_id")]
        public int? CenterId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        [JsonProperty("state_name")]
        public string StateName { get; set; }

        [JsonProperty("district_name")]
        public string DistrictName { get; set; }

        [JsonProperty("block_name")]
        public string BlockName { get; set; }

        [JsonProperty("pincode")]
        public string Pincode { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        [JsonProperty("fee_type")]
        public string FeeType { get; set; }

        public List<AppointmentSession> Sessions { get; set; }

        public AppointmentCenter() =>
            this.Sessions = new List<AppointmentSession>();

        public bool IsAvailable(int age)
        {
            if (this.Sessions.IsNullOrEmpty())
            {
                return false;
            }

            foreach (var session in this.Sessions)
            {
                if (session.MinimumAgeLimit > age)
                {
                    continue;
                }

                if (session.AvaialableCapacityDose1.HasValue
                    && session.AvaialableCapacityDose1.Value > 0)
                {
                    return true;
                }

                // ToDo: Enable this check when
                // you also want to check for Dose 2.
                //if (session.AvaialableCapacityDose2.HasValue
                //    && session.AvaialableCapacityDose2.Value > 0)
                //{
                //    return true;
                //}
            }

            return false;
        }
    }
}
