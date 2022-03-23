using System.Threading.Tasks;

namespace Sol.Unity.Wallet.Bip39
{
    /// <summary>
    /// Specifies functionality for the wordlist source.
    /// </summary>
    internal interface IWordlistSource
    {
        /// <summary>
        /// Load the wordlist.
        /// </summary>
        /// <param name="name">The name of the wordlist.</param>
        /// <returns>A task that returns the wordlist.</returns>
        Task<WordList> LoadAsync(string name);
    }
}