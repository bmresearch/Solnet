using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Core.Sockets;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;

namespace Solnet.Programs.Models
{
    /// <summary>
    /// Wraps a result to an RPC request.
    /// </summary>
    /// <typeparam name="T">The underlying type of the request.</typeparam>
    /// <typeparam name="T2">The underlying type of the request.</typeparam>
    public class ResultWrapper<T, T2>
    {

        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        public ResultWrapper(RequestResult<T> result)
        {
            OriginalRequest = result;
        }

        /// <summary>
        /// Initialize the result wrapper with the given result and it's parsed result type.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <param name="parsedResult">The parsed result type.</param>
        public ResultWrapper(RequestResult<T> result, T2 parsedResult)
        {
            OriginalRequest = result;
            ParsedResult = parsedResult;
        }

        /// <summary>
        /// The original response to the request.
        /// </summary>
        public RequestResult<T> OriginalRequest { get; init; }

        /// <summary>
        /// The desired type of the account data.
        /// </summary>
        public T2 ParsedResult { get; set; }

        /// <summary>
        /// Whether the deserialization of the account data into the desired structure was successful.
        /// </summary>
        public bool WasDeserializationSuccessful => ParsedResult != null;

        /// <summary>
        /// Whether the original request and the deserialization of the account data into the desired structure was successful.
        /// </summary>
        public bool WasSuccessful => OriginalRequest.WasSuccessful && WasDeserializationSuccessful;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MultipleAccountsResultWrapper<T> : ResultWrapper<ResponseValue<List<AccountInfo>>, T>
    {
        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        public MultipleAccountsResultWrapper(RequestResult<ResponseValue<List<AccountInfo>>> result) : base(result) { }

        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <param name="parsedResult">The parsed result type.</param>
        public MultipleAccountsResultWrapper(RequestResult<ResponseValue<List<AccountInfo>>> result, T parsedResult) : base(result, parsedResult) { }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AccountResultWrapper<T> : ResultWrapper<ResponseValue<AccountInfo>, T>
    {
        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        public AccountResultWrapper(RequestResult<ResponseValue<AccountInfo>> result) : base(result) { }

        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <param name="parsedResult">The parsed result type.</param>
        public AccountResultWrapper(RequestResult<ResponseValue<AccountInfo>> result, T parsedResult) : base(result, parsedResult) { }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ProgramAccountsResultWrapper<T> : ResultWrapper<List<AccountKeyPair>, T>
    {
        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        public ProgramAccountsResultWrapper(RequestResult<List<AccountKeyPair>> result) : base(result) { }

        /// <summary>
        /// Initialize the result wrapper with the given result.
        /// </summary>
        /// <param name="result">The result of the request.</param>
        /// <param name="parsedResult">The parsed result type.</param>
        public ProgramAccountsResultWrapper(RequestResult<List<AccountKeyPair>> result, T parsedResult) : base(result, parsedResult) { }
    }

    /// <summary>
    /// Wraps the base subscription to have the underlying data of the subscription, which is sometimes needed to perform
    /// some logic before returning data to the subscription caller.
    /// </summary>
    /// <typeparam name="T">The type of the subscription.</typeparam>
    public class SubscriptionWrapper<T> : Subscription
    {
        /// <summary>
        /// The underlying data.
        /// </summary>
        public T Data;
    }

    /// <summary>
    /// Wraps a subscription with a generic type to hold either order book or trade events.
    /// </summary>
    public class Subscription
    {
        /// <summary>
        /// The address associated with this data.
        /// </summary>
        public PublicKey Address;

        /// <summary>
        /// The underlying subscription state.
        /// </summary>
        public SubscriptionState SubscriptionState;
    }
}