namespace KO.Covid.Application.Models
{
    using Newtonsoft.Json;

    public class ConfirmOtpRequest
    {
        public string Otp { get; set; }

        [JsonProperty("txnId")]
        public string TransactionId { get; set; }
    }
}
