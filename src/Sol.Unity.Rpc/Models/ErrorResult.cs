using System.Text.Json.Serialization;

namespace Sol.Unity.Rpc.Models
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
        public TransactionError Error { get; set; }
    }
}