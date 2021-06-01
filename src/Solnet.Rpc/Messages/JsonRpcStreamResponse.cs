using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Messages
{
    public class JsonRpcStreamResponse <T>
    {
        public T Result { get; set; }

        public int Subscription { get; set; }
    }
}
