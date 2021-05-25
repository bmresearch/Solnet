using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public abstract class SubscriptionState
    {
        private readonly SolanaStreamingRpcClient _rpcClient;
        internal int SubscriptionId { get; set; }
        public SubscriptionChannel Channel { get; }

        public SubscriptionStatus State { get; private set; }

        private ImmutableList<object> AdditionalParameters { get; }

        public event EventHandler<SubscriptionEvent> SubscriptionChanged;

        internal SubscriptionState(SolanaStreamingRpcClient rpcClient, SubscriptionChannel chan, IList<object> aditionalParameters = default)
        {
            _rpcClient = rpcClient;
            Channel = chan;
            AdditionalParameters = aditionalParameters?.ToImmutableList();
        }

        internal void ChangeState(SubscriptionStatus newState, string error = null, string code = null)
        {
            State = newState;
            SubscriptionChanged?.Invoke(this, new SubscriptionEvent(newState, error, code));
        }

        internal abstract void HandleData(object data);

        public void Unsubscribe() => _rpcClient.Unsubscribe(this);
        public async Task UnsubscribeAsync() => await _rpcClient.UnsubscribeAsync(this).ConfigureAwait(false);
    }

    internal class SubscriptionState<T> : SubscriptionState
    {
        internal Action<SubscriptionState<T>, T> DataHandler;

        internal SubscriptionState(SolanaStreamingRpcClient rpcClient, SubscriptionChannel chan, Action<SubscriptionState, T> handler, IList<object> aditionalParameters = default)
            : base(rpcClient, chan, aditionalParameters)
        {
            DataHandler = handler;
        }

        internal override void HandleData(object data) => DataHandler(this, (T)data);
    }
}
