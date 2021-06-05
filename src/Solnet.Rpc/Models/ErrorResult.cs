using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    public class ErrorResult
    {
        [JsonPropertyName("err")]
        public string Error { get; set; }
    }
}