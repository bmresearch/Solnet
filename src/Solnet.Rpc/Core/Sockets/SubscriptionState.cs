using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    /// <summary>
    /// Represents the state of a given subscription.
    /// </summary>
    public abstract class SubscriptionState
    {
        /// <summary>
        /// Streaming client reference for easy unsubscription.
        /// </summary>
        private readonly IStreamingRpcClient _rpcClient;

        /// <summary>
        /// The subscription ID as confirmed by the node.
        /// </summary>
        internal int SubscriptionId { get; set; }

        /// <summary>
        /// The channel subscribed.
        /// </summary>
        public SubscriptionChannel Channel { get; protected set; }

        /// <summary>
        /// The current state of the subscription.
        /// </summary>
        public SubscriptionStatus State { get; protected set; }

        /// <summary>
        /// The last error message.
        /// </summary>
        public string LastError { get; protected set; }

        /// <summary>
        /// The last error code.
        /// </summary>
        public string LastCode { get; protected set; }

        /// <summary>
        /// The collection of parameters that were submitted for this subscription.
        /// </summary>
        public ImmutableList<object> AdditionalParameters { get; protected set; }

        /// <summary>
        /// Event fired when the state of the subcription changes.
        /// </summary>
        public event EventHandler<SubscriptionEvent> SubscriptionChanged;

        /// <summary>
        /// Base constructor.
        /// </summary>
        /// <param name="rpcClient">The streaming rpc client reference.</param>
        /// <param name="chan">The channel of this subscription.</param>
        /// <param name="additionalParameters">Additional parameters for this given subscription.</param>
        protected SubscriptionState(IStreamingRpcClient rpcClient, SubscriptionChannel chan, IList<object> additionalParameters = default)
        {
            _rpcClient = rpcClient;
            Channel = chan;
            AdditionalParameters = additionalParameters?.ToImmutableList();
        }

        /// <summary>
        /// Default constructor to help setup tests.
        /// </summary>
        protected SubscriptionState() { }

        /// <summary>
        /// Changes the state of the subscription and invokes the event.
        /// </summary>
        /// <param name="newState">The new state of the subscription.</param>
        /// <param name="error">The possible error message.</param>
        /// <param name="code">The possible error code.</param>
        internal void ChangeState(SubscriptionStatus newState, string error = null, string code = null)
        {
            State = newState;
            LastError = error;
            LastCode = code;
            SubscriptionChanged?.Invoke(this, new SubscriptionEvent(newState, error, code));
        }

        /// <summary>
        /// Invokes the data handler.
        /// </summary>
        /// <param name="data">The data.</param>
        protected internal abstract void HandleData(object data);

        /// <summary>
        /// Unsubscribes the current subscription.
        /// </summary>
        public void Unsubscribe() => _rpcClient.Unsubscribe(this);

        /// <inheritdoc cref="Unsubscribe"/>
        public async Task UnsubscribeAsync() => await _rpcClient.UnsubscribeAsync(this).ConfigureAwait(false);
    }

    /// <summary>
    /// Represents the state of a given subscription with specified type handler.
    /// </summary>
    /// <typeparam name="T">The type of the data received by this subscription.</typeparam>
    internal class SubscriptionState<T> : SubscriptionState
    {
        /// <summary>
        /// The data handler reference.
        /// </summary>
        internal Action<SubscriptionState<T>, T> DataHandler;

        /// <summary>
        /// Constructor with all parameters related to a given subscription.
        /// </summary>
        /// <param name="rpcClient">The streaming rpc client reference.</param>
        /// <param name="chan">The channel of this subscription.</param>
        /// <param name="handler">The handler for the data received.</param>
        /// <param name="additionalParameters">Additional parameters for this given subscription.</param>
        internal SubscriptionState(SolanaStreamingRpcClient rpcClient, SubscriptionChannel chan, Action<SubscriptionState, T> handler, IList<object> additionalParameters = default)
            : base(rpcClient, chan, additionalParameters)
        {
            DataHandler = handler;
        }

        /// <inheritdoc cref="SubscriptionState.HandleData(object)"/>
        protected internal override void HandleData(object data) => DataHandler(this, (T)data);
    }
}