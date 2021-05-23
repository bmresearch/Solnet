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
        public SubscriptionChannel Channel { get; }

        public event EventHandler<SubscriptionEvent> SubscriptionChanged;

        public ImmutableList<object> AdditionalParameters { get; }

        internal void RaiseEvent(object sender, SubscriptionEvent e) => SubscriptionChanged?.Invoke(sender, e);

        internal abstract void HandleData(object data);

        internal SubscriptionState(SubscriptionChannel chan, IList<object> aditionalParameters = default)
        {
            Channel = chan;
            AdditionalParameters = aditionalParameters?.ToImmutableList();
        }
    }

    internal class SubscriptionState<T> : SubscriptionState
    {
        internal Action<SubscriptionState<T>, T> DataHandler;

        internal SubscriptionState(SubscriptionChannel chan, Action<SubscriptionState, T> handler) : base(chan)
        {
            DataHandler = handler;
        }

        internal override void HandleData(object data) => DataHandler(this, (T)data);
    }
}
