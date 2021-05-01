using Solnet.Rpc.NonceService;
using Solnet.Rpc.TransactionManagers;

namespace Solnet.RPC.Accounts
{
    /// <summary>
    /// Specifies the Account interface.
    /// </summary>
    public interface IAccount
    {
        /// <summary>
        /// The address.
        /// </summary>
        string Address { get; }
        
        /// <summary>
        /// The transaction manager.
        /// </summary>
        ITransactionManager TransactionManager { get; }

        /// <summary>
        /// The nonce service.
        /// </summary>
        INonceService NonceService { get; set; }
    }
}