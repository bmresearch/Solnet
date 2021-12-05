using Solnet.Wallet;

namespace Solnet.Programs.TokenSwap.Models
{

    /// <summary>
    /// TokenSwap program state
    /// </summary>
    public class TokenSwapAccount 
    {
        /// <summary>
        /// the size of this account in bytes
        /// </summary>
        public const int TOKEN_SWAP_DATA_LEN = 324;

        /// <summary>
        /// Versions of this state account
        /// </summary>
        public enum SwapVersion { SwapV1 = 1 }

        /// <summary>
        /// Version of this state account
        /// </summary>
        public SwapVersion Version;

        /// <summary>
        /// Initialized state
        /// </summary>
        public bool IsInitialized;

        /// <summary>
        /// Nonce used in program address.
        /// The program address is created deterministically with the nonce,
        /// swap program id, and swap account pubkey.  This program address has
        /// authority over the swap's token A account, token B account, and pool
        /// token mint.
        /// </summary>
        public byte Nonce;

        /// <summary>
        /// Program ID of the tokens being exchanged.
        /// </summary>
        public PublicKey TokenProgramId;

        /// <summary>
        /// Token A
        /// </summary>
        public PublicKey TokenAAccount;

        /// <summary>
        /// Token B
        /// </summary>
        public PublicKey TokenBAccount;

        /// <summary>
        /// Pool tokens are issued when A or B tokens are deposited.
        /// Pool tokens can be withdrawn back to the original A or B token.
        /// </summary>
        public PublicKey PoolMint;

        /// <summary>
        /// Mint information for token A
        /// </summary>
        public PublicKey TokenAMint;

        /// <summary>
        /// Mint information for token B
        /// </summary>
        public PublicKey TokenBMint;

        /// <summary>
        /// Pool token account to receive trading and / or withdrawal fees
        /// </summary>
        public PublicKey PoolFeeAccount;

        /// <summary>
        /// All fee information
        /// </summary>
        public Fees Fees;

        /// <summary>
        /// Swap curve parameters, to be unpacked and used by the SwapCurve, which
        /// calculates swaps, deposits, and withdrawals
        /// </summary>
        public SwapCurve SwapCurve;

        /// <summary>
        /// Deserilize a token swap from the bytes of an account
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TokenSwapAccount Deserialize(byte[] data)
        {
            if (data.Length != TOKEN_SWAP_DATA_LEN)
                return null;

            var ret = new TokenSwapAccount()
            {
                Version = SwapVersion.SwapV1,
                IsInitialized = data[1] == 1,
                Nonce = data[2],
                TokenProgramId = new PublicKey(data[3..35]),
                TokenAAccount = new PublicKey(data[35..67]),
                TokenBAccount = new PublicKey(data[67..99]),
                PoolMint = new PublicKey(data[99..131]),
                TokenAMint = new PublicKey(data[131..163]),
                TokenBMint = new PublicKey(data[163..195]),
                PoolFeeAccount = new PublicKey(data[195..227]),
                Fees = Fees.Deserialize(data[227..291]),
                SwapCurve = SwapCurve.Deserialize(data[291..]),
            };
            return ret;
        }
    }
}
