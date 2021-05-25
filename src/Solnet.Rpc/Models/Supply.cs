using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    public class Supply
    {
        /// <summary>
        /// Circulating supply in lamports.
        /// </summary>
        public ulong Circulating { get; set; }
        
        /// <summary>
        /// Non-circulating supply in lamports.
        /// </summary>
        public ulong NonCirculating { get; set; }
        
        /// <summary>
        /// A list of account addresses of non-circulating accounts, as strings.
        /// </summary>
        public IList<string> NonCirculatingAccounts { get; set; }
        
        /// <summary>
        /// Total supply in lamports.
        /// </summary>
        public ulong Total { get; set; }
    }
}