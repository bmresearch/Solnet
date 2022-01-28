// ReSharper disable ClassNeverInstantiated.Global
using System;
using System.Globalization;

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
    /// Represents the account info for a given token account.
    /// </summary>
    public class TokenMintInfo : AccountInfoBase
    {
        /// <summary>
        /// The parsed token account data field.
        /// </summary>
        public TokenMintData Data { get; set; }
    }

    /// <summary>
    /// Represents a Token Mint account data.
    /// </summary>
    public class TokenMintData
    {
        /// <summary>
        /// The program responsible for the account data.
        /// </summary>
        public string Program { get; set; }

        /// <summary>
        /// Account data space.
        /// </summary>
        public ulong Space { get; set; }

        /// <summary>
        /// The parsed token mint data.
        /// </summary>
        public ParsedTokenMintData Parsed { get; set; }
    }

    /// <summary>
    /// Represents the Token Mint parsed data, as formatted per SPL token program.
    /// </summary>
    public class ParsedTokenMintData
    {
        /// <summary>
        /// Contains the details of the token mint.
        /// </summary>
        public TokenMintInfoDetails Info { get; set; }

        /// <summary>
        /// The type of the account managed by the SPL token program.
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Represents a Token Mint account info as formatted per the SPL token program.
    /// </summary>
    public class TokenMintInfoDetails
    {
        /// <summary>
        /// The freeze authority.
        /// </summary>
        public string FreezeAuthority { get; set; }

        /// <summary>
        /// The mint authority.
        /// </summary>
        public string MintAuthority { get; set; }

        /// <summary>
        /// The decimals cases to consider when converter to human readable token amounts.
        /// </summary>
        public byte Decimals { get; set; }

        /// <summary>
        /// Is the mint account initialized?
        /// </summary>
        public bool IsInitialized { get; set; }

        /// <summary>
        /// The current token supply.
        /// </summary>
        public string Supply { get; set; }

        /// <summary>
        /// The current token supply parsed as ulong.
        /// </summary>
        public ulong SupplyUlong => ulong.Parse(Supply);
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
        /// The token balance that has been delegated.
        /// </summary>
        public TokenBalance DelegatedAmount { get; set; }

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
        /// Account data space.
        /// </summary>
        public ulong Space { get; set; }

        /// <summary>
        /// The parsed account data, as available by the program-specific state parser.
        /// </summary>
        public ParsedTokenAccountData Parsed { get; set; }
    }

    /// <summary>
    /// Represents a large token account.
    /// </summary>
    public class LargeTokenAccount : TokenBalance
    {
        /// <summary>
        /// The address of the token account.
        /// </summary>
        public string Address { get; set; }
    }

    /// <summary>
    /// Represents a large account.
    /// </summary>
    public class LargeAccount
    {
        /// <summary>
        /// The lamports balance of the account.
        /// </summary>
        public ulong Lamports { get; set; }


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

        /// <summary>
        /// The token account balance as a ulong
        /// </summary>
        public ulong AmountUlong => Convert.ToUInt64(Amount);

        /// <summary>
        /// The token account balance as a decimal
        /// </summary>
        public decimal AmountDecimal => Convert.ToDecimal(UiAmountString, CultureInfo.InvariantCulture);

        /// <summary>
        /// The token account balance as a double
        /// </summary>
        public double AmountDouble => Convert.ToDouble(UiAmountString, CultureInfo.InvariantCulture);
    }
}