namespace KO.Covid.Application.Models
{
    using Newtonsoft.Json;

    public class GenerateOtpRequest
    {
        [JsonProperty("mobile")]
        public string Mobile { get; set; }
    }
}
