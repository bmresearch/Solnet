using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Models
{
    public class Authorized
    {
        public PublicKey staker { get; set; }
        public PublicKey withdrawer { get; set; }
    }
}
