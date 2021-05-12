namespace KO.Covid.Application.Models
{
    using Newtonsoft.Json;

    public class GenerateOtpResponse : ErrorResponse
    {
        [JsonProperty("txnId")]
        public string TransactionId { get; set; }
    }
}
