using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    public interface IMintResolver
    {
        MintInfo Resolve(string mint);
    }
}
