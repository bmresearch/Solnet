using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AccountInfo
    {
        public ulong Lamports { get; set; }

        public string Owner { get; set; }

        public bool Executable { get; set; }

        public ulong RentEpoch { get; set; }

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