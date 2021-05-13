namespace KO.Covid.Application.Models
{
    using Newtonsoft.Json;

    public class GenerateOtpResponse
    {
        [JsonProperty("txnId")]
        public string TransactionId { get; set; }
    }
}
