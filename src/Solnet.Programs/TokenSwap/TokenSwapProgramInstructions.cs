using System.Collections.Generic;

namespace Solnet.Programs.TokenSwap
{
    /// <summary>
    /// Represents the instruction types for the <see cref="TokenSwapProgram"/> along with a friendly name so as not to use reflection.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/token-swap
    /// https://docs.rs/spl-token-swap/2.1.0/spl_token_swap/
    /// </remarks>
    /// </summary>
    internal static class TokenSwapProgramInstructions
    {
        /// <summary>
        /// Represents the user-friendly names for the instruction types for the <see cref="TokenSwapProgram"/>.
        /// </summary>
        internal static readonly Dictionary<Values, string> Names = new()
        {
            { Values.Initialize, "Initialize Swap" },
            { Values.Swap, "Swap" },
            { Values.DepositAllTokenTypes, "Deposit Both" },
            { Values.WithdrawAllTokenTypes, "Withdraw Both" },
            { Values.DepositSingleTokenTypeExactAmountIn, "Deposit Single" },
            { Values.WithdrawSingleTokenTypeExactAmountOut, "Withdraw Single" },
        };

        /// <summary>
        /// Represents the instruction types for the <see cref="TokenSwapProgram"/>.
        /// </summary>
        internal enum Values : byte
        {
            /// <summary>
            /// Initializes a new swap.
            /// </summary>
            Initialize = 0,

            /// <summary>
            /// Swap the tokens in the pool.
            /// </summary>
            Swap = 1,

            /// <summary>
            /// Deposit both types of tokens into the pool.
            /// </summary>
            DepositAllTokenTypes = 2,

            /// <summary>
            /// Withdraw both types of tokens from the pool at the current ratio.
            /// </summary>
            WithdrawAllTokenTypes = 3,

            /// <summary>
            /// Deposit one type of tokens into the pool.
            /// </summary>
            DepositSingleTokenTypeExactAmountIn = 4,

            /// <summary>
            /// Withdraw one token type from the pool at the current ratio.
            /// </summary>
            WithdrawSingleTokenTypeExactAmountOut = 5,

        }
    }
}