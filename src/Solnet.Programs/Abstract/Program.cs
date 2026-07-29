using Solnet.Wallet;

namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// Base Program interface.
    /// </summary>
    public interface IProgram
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