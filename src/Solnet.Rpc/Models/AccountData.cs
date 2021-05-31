// ReSharper disable ClassNeverInstantiated.Global
using System;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the account info for a given token account.
    /// </summary>
    public class TokenAccountInfo : AccountInfoBase
    {
        /// <summary>
        /// The parsed token account data field.
        /// </summary>
        public TokenAccountData Data { get; set; }
    }

    /// <summary>
    /// Represents the details of the info field of a token account.
    /// </summary>
    public class TokenAccountInfoDetails
    {
        /// <summary>
        /// The token balance data.
        /// </summary>
        public TokenBalance TokenAmount { get; set; }

        /// <summary>
        /// A base-58 encoded public key of the delegate.
        /// </summary>
        public string Delegate { get; set; }

        /// <summary>
        /// The delegated amount of tokens.
        /// </summary>
        public ulong DelegatedAmount { get; set; }

        /// <summary>
        /// The account's state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// If the account is a native token account.
        /// </summary>
        public bool IsNative { get; set; }

        /// <summary>
        /// A base-58 encoded public key of the token's mint.
        /// </summary>
        public string Mint { get; set; }

        /// <summary>
        /// A base-58 encoded public key of the program this account as been assigned to.
        /// </summary>
        public string Owner { get; set; }
    }

    /// <summary>
    /// Represents the parsed account data, as available by the program-specific state parser.
    /// </summary>
    public class ParsedTokenAccountData
    {
        /// <summary>
        /// The type of account.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The token account info, containing account balances, delegation and ownership info.
        /// </summary>
        public TokenAccountInfoDetails Info { get; set; }
    }

    /// <summary>
    /// Represents a token account's data.
    /// </summary>
    public class TokenAccountData
    {
        /// <summary>
        /// The program responsible for the account data.
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// The parsed account data, as available by the program-specific state parser.
        /// </summary>
        public ParsedTokenAccountData Parsed { get; set; }
    }

    /// <summary>
    /// Represents a large token account.
    /// </summary>
    public class LargeAccount : TokenBalance
    {
        /// <summary>
        /// The address of the token account.
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// Represents the token balance of an account.
    /// </summary>
    public class TokenBalance
    {
        /// <summary>
        /// The raw token account balance without decimals.
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// The number of base 10 digits to the right of the decimal place.
        /// </summary>
        public int Decimals { get; set; }

        /// <summary>
        /// The token account balance, using mint-prescribed decimals. DEPRECATED.
        /// </summary>
        [Obsolete("UiAmount is deprecated, please use UiAmountString instead.")]
        public decimal? UiAmount { get; set; }

        /// <summary>
        /// The token account balance as a string, using mint-prescribed decimals.
        /// </summary>
        public string UiAmountString { get; set; }
    }
}