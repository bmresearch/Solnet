using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Models
{
    public class SlotInfo
    {
        public int Parent { get; set; }

        public int Root { get; set; }

        public int Slot { get; set; }

    }
}
