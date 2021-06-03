namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the identity public key for the current node.
    /// </summary>
    public class NodeIdentity
    {
        /// <summary>
        /// The identity public key of the current node, as base-58 encoded string.
        /// </summary>
        public string Identity { get; set; }
    }
}