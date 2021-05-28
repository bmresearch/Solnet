using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Types
{
    public enum Commitment
    {
        Finalized,
        Confirmed,
        Processed
    }
}
