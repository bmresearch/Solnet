// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the account info.
    /// </summary>
    public class AccountInfo
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

        /// <summary>
        /// The actual account data.
        /// <remarks>
        /// This value is accessed via <see cref="TryGetAccountData(out string)"/> or <see cref="TryGetAccountData(out Solnet.Rpc.Models.TokenAccountData)"/>
        /// in order to get the data according to the underlying encoding type requested, see <see cref="Types.BinaryEncoding"/>.
        /// </remarks>
        /// </summary>
        [JsonConverter(typeof(AccountDataJsonConverter))]
        public object Data { get; set; }

        /// <summary>
        /// Tries to retrieve the Account data as a TokenAccountData encoded object.
        /// </summary>
        /// <param name="accData">The account data.</param>
        /// <returns>Returns true if the account data was foud as a TokenAccountData object, false otherwise.</returns>
        public bool TryGetAccountData(out TokenAccountData accData)
        {
            accData = (TokenAccountData)Data;
            return accData != null;
        }

        /// <summary>
        /// Tries to retrieve the Account data as a base64 encoded string.
        /// </summary>
        /// <param name="accData">The account data.</param>
        /// <returns>Returns true if the account data was foud as a string, false otherwise.</returns>
        public bool TryGetAccountData(out string accData)
        {
            accData = null;
            var list = (IList<string>)Data;
            if(list.Count > 0)
            {
                accData = list[0];
                return true;
            }
            return false;
        }
    }
}