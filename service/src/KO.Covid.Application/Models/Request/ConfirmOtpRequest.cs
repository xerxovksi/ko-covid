namespace KO.Covid.Application.Models
{
    using Newtonsoft.Json;

    public class ConfirmOtpRequest
    {
        [JsonProperty("otp")]
        public string Otp { get; set; }

        [JsonProperty("txnId")]
        public string TransactionId { get; set; }
    }
}
