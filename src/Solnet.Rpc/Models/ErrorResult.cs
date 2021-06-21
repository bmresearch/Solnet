using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Holds an error result.
    /// </summary>
    public class ErrorResult
    {
        /// <summary>
        /// The error string.
        /// </summary>
        [JsonPropertyName("err")]
        public string Error { get; set; }
    }
}