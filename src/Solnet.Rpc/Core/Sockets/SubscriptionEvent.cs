using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Sockets
{
    public class SubscriptionEvent : EventArgs
    {
        public SubscriptionStatus Status { get; }

        public string Error { get; }

        public string Code { get; }

        internal SubscriptionEvent(SubscriptionStatus status, string error = default, string code = default)
        {
            Status = status;
            Error = error;
            Code = code;
        }
    }
}
