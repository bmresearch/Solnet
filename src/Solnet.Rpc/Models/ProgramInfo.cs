using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Models
{
    public class ProgramInfo
    {
        public string PubKey { get; set; }

        public AccountInfo<IList<string>> Account { get; set; }
    }
}
