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
        private SolanaStreamingClient _client;
        internal int SubscriptionId { get; set; }
        public SubscriptionChannel Channel { get; }

        public SubscriptionStatus State { get; private set; }

        public ImmutableList<object> AdditionalParameters { get; }

        public event EventHandler<SubscriptionEvent> SubscriptionChanged;

        internal SubscriptionState(SolanaStreamingClient client, SubscriptionChannel chan, IList<object> aditionalParameters = default)
        {
            _client = client;
            Channel = chan;
            AdditionalParameters = aditionalParameters?.ToImmutableList();
        }

        internal void ChangeState(SubscriptionStatus newState, string error = null, string code = null)
        {
            State = newState;
            SubscriptionChanged?.Invoke(this, new SubscriptionEvent(newState, error, code));
        }

        internal abstract void HandleData(object data);

        public void Unsubscribe() => _client.Unsubscribe(this);
        public async Task UnsubscribeAsync() => await _client.UnsubscribeAsync(this).ConfigureAwait(false);
    }

    internal class SubscriptionState<T> : SubscriptionState
    {
        internal Action<SubscriptionState<T>, T> DataHandler;

        internal SubscriptionState(SolanaStreamingClient client, SubscriptionChannel chan, Action<SubscriptionState, T> handler, IList<object> aditionalParameters = default)
            : base(client, chan, aditionalParameters)
        {
            DataHandler = handler;
        }

        internal override void HandleData(object data) => DataHandler(this, (T)data);
    }
}
