using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the signature status information.
    /// </summary>
    public class SignatureStatusInfo
    {
        /// <summary>
        /// The slot the transaction was processed in.
        /// </summary>
        public ulong Slot { get; set; }
        
        /// <summary>
        /// The number of blocks since signature confirmation.
        /// </summary>
        public ulong? Confirmations { get; set; }
        
        /// <summary>
        /// The error if the transaction failed, null if it succeeded.
        /// </summary>
        [JsonPropertyName("err")]
        public TransactionError Error { get; set; }
        
        /// <summary>
        /// The transaction's cluster confirmation status, either "processed", "confirmed" or "finalized".
        /// </summary>
        public string ConfirmationStatus { get; set; }
    }
}