namespace Sol.Unity.Rpc.Messages
{
    /// <summary>
    /// Holds a json rpc message from a streaming socket.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonRpcStreamResponse<T>
    {
        /// <summary>
        /// The message received.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// The subscription id that the message belongs to.
        /// </summary>
        public int Subscription { get; set; }
    }
}