namespace Solnet.Rpc.Messages
{
    /// <summary>
    /// Base JpnRpc message.
    /// </summary>
    public class JsonRpcBase
    {
        /// <summary>
        /// The rpc version.
        /// </summary>
        public string Jsonrpc { get; protected set; }

        /// <summary>
        /// The id of the message.
        /// </summary>
        public int Id { get; set; }
    }
}