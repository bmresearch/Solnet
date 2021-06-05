// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// The base class of the account info, to be subclassed for token a account info classes.
    /// </summary>
    public class AccountInfoBase
    {
        /// <summary>
        /// The lamports balance of the account.
        /// </summary>
        public ulong Lamports { get; set; }

        /// <summary>
        /// The account owner.
        /// <remarks>
        /// This value could be another regular address or a program.
        /// </remarks>
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Indicates whether the account contains a program (and is strictly read-only).
        /// </summary>
        public bool Executable { get; set; }

        /// <summary>
        /// The epoch at which the account will next owe rent.
        /// </summary>
        public ulong RentEpoch { get; set; }
    }

    /// <summary>
    /// Represents the account info.
    /// </summary>
    public class AccountInfo : AccountInfoBase
    {
        /// <summary>
        /// The actual account data.
        /// <remarks>
        /// This field should contain two values: first value is the data, the second one is the encoding - should always read base64.
        /// </remarks>
        /// </summary>
        public List<string> Data { get; set; }
    }

    /// <summary>
    /// Represents the tuple account key and account data.
    /// </summary>
    public class AccountKeyPair
    {
        /// <summary>
        /// The account info.
        /// </summary>
        public AccountInfo Account { get; set; }

        /// <summary>
        /// A base-58 encoded public key representing the account's public key.
        /// </summary>
        [JsonPropertyName("pubkey")]
        public string PublicKey { get; set; }
    }

}