using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Abstract
{
    public interface Program
    {
        /// <summary>
        /// The program's key
        /// </summary>
        PublicKey ProgramIdKey { get; }
        /// <summary>
        /// The name of the program
        /// </summary>
        string ProgramName { get; }
    }
}
