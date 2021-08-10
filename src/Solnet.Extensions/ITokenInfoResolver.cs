using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Extensions.TokenInfo
{
    public interface ITokenInfoResolver
    {
        TokenDef Resolve(string mint);
    }
}
