using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// A class to abstract some of the core program commonality
    /// </summary>
    public abstract class BaseProgram : Program
    {
        private PublicKey _programIdKey;
        private string _programName;

        /// <summary>
        /// The public key of the program.
        /// </summary>
        public virtual PublicKey ProgramIdKey => _programIdKey;

        /// <summary>
        /// The program's name.
        /// </summary>
        public virtual string ProgramName => _programName;

        /// <summary>
        /// Creates an instance of the base program class with specified id and name
        /// </summary>
        /// <param name="programIdKey">The program key</param>
        /// <param name="programName">The program name</param>
        protected BaseProgram(PublicKey programIdKey, string programName)
        {
            _programIdKey = programIdKey;
            _programName = programName;
        }
    }
}
