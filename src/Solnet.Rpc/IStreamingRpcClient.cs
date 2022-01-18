using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// Represents the streaming RPC client for the solana API.
    /// </summary>
    public interface IStreamingRpcClient : IDisposable
    {
        /// <summary>
        /// Current connection state.
        /// </summary>
        WebSocketState State { get; }

        /// <summary>
        /// Event triggered when the connection status changes between connected and disconnected.
        /// </summary>
        event EventHandler<WebSocketState> ConnectionStateChangedEvent;

        /// <summary>
        /// The address this client connects to.
        /// </summary>
        Uri NodeAddress { get; }

        /// <summary>
        /// Statistics of the current connection.
        /// </summary>
        IConnectionStatistics Statistics { get; }

        /// <summary>
        /// Subscribes asynchronously to AccountInfo notifications.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeAccountInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the AccountInfo. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeAccountInfo(string pubkey, Action<SubscriptionState, ResponseValue<AccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to Token Account notifications. Note: Only works if the account is a Token Account.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeTokenAccountAsync(string pubkey, Action<SubscriptionState, ResponseValue<TokenAccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes  to Token Account notifications. Note: Only works if the account is a Token Account.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key of the account.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeTokenAccount(string pubkey, Action<SubscriptionState, ResponseValue<TokenAccountInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to the logs notifications that mention a given public key.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key to filter by mention.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeLogInfoAsync(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to the logs notifications.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="subscriptionType">The filter mechanism.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeLogInfoAsync(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the logs notifications that mention a given public key. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="pubkey">The public key to filter by mention.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeLogInfo(string pubkey, Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to the logs notifications. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="subscriptionType">The filter mechanism.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeLogInfo(LogsSubscriptionType subscriptionType, Action<SubscriptionState, ResponseValue<LogInfo>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to a transaction signature to receive notification when the transaction is confirmed.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="transactionSignature">The transaction signature.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeSignatureAsync(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to a transaction signature to receive notification when the transaction is confirmed. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="transactionSignature">The transaction signature.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeSignature(string transactionSignature, Action<SubscriptionState, ResponseValue<ErrorResult>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to changes to a given program account data.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="programPubkey">The program pubkey.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeProgramAsync(string programPubkey, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes to changes to a given program account data. This is a synchronous and blocking function.
        /// </summary>
        /// <remarks>
        /// The <c>commitment</c> parameter is optional, the default value <see cref="Commitment.Finalized"/> is not sent.
        /// </remarks>
        /// <param name="programPubkey">The program pubkey.</param>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeProgram(string programPubkey, Action<SubscriptionState, ResponseValue<AccountKeyPair>> callback, Commitment commitment = Commitment.Finalized);

        /// <summary>
        /// Subscribes asynchronously to receive notifications anytime a slot is processed by the validator.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeSlotInfoAsync(Action<SubscriptionState, SlotInfo> callback);

        /// <summary>
        /// Subscribes to receive notifications anytime a slot is processed by the validator. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeSlotInfo(Action<SubscriptionState, SlotInfo> callback);


        /// <summary>
        /// Subscribes asynchronously to receive notifications anytime a new root is set by the validator.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<SubscriptionState> SubscribeRootAsync(Action<SubscriptionState, int> callback);

        /// <summary>
        /// Subscribes to receive notifications anytime a new root is set by the validator. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="callback">The callback to handle data notifications.</param>
        /// <returns>Returns an object representing the state of the subscription.</returns>
        SubscriptionState SubscribeRoot(Action<SubscriptionState, int> callback);

        /// <summary>
        /// Asynchronously unsubscribes from a given subscription using the state object.
        /// </summary>
        /// <param name="subscription">The subscription state object.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task UnsubscribeAsync(SubscriptionState subscription);

        /// <summary>
        /// Unsubscribes from a given subscription using the state object. This is a synchronous and blocking function.
        /// </summary>
        /// <param name="subscription">The subscription state object.</param>
        void Unsubscribe(SubscriptionState subscription);

        /// <summary>
        /// Asynchronously initializes the client connection and starts listening for socket messages.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task ConnectAsync();
        /// <summary>
        /// Asynchronously disconnects and removes all running subscriptions.
        /// </summary>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task DisconnectAsync();
    }
}