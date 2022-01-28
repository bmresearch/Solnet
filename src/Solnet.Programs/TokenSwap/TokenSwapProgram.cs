using Solnet.Programs.Abstract;
using Solnet.Programs.TokenSwap.Models;
using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs.TokenSwap
{
    /// <summary>
    /// Implements the Token Swap Program methods.
    /// <remarks>
    /// For more information see:
    /// https://spl.solana.com/token-swap
    /// https://docs.rs/spl-token-swap/2.1.0/spl_token_swap/
    /// </remarks>
    /// </summary>
    public class TokenSwapProgram : BaseProgram
    {
        /// <summary>
        /// SPL Token Swap Program Program ID
        /// </summary>
        public static readonly PublicKey TokenSwapProgramIdKey = new("SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8");

        /// <summary>
        /// SPL Token Swap Program Program Name
        /// </summary>
        public static readonly string TokenSwapProgramName = "Token Swap Program";

        
        //instance vars

        /// <summary>
        /// The owner key required to use as the fee account owner.  
        /// </summary>
        public virtual PublicKey OwnerKey => new("HfoTxFR1Tm6kGmWgYWD6J7YHVy1UwqSULUGVLXkJqaKN");

        /// <summary>
        /// Token Swap account layout size.
        /// </summary>
        public static readonly ulong TokenSwapAccountDataSize = 323;
        
        /// <summary>
        /// Create a token swap program instance with the standard programid and program name
        /// </summary>
        public TokenSwapProgram() : base(TokenSwapProgramIdKey, TokenSwapProgramName) { }

        /// <summary>
        /// Create a token swap program instance with a custom programid 
        /// </summary>
        /// <param name="programId">The program id to use</param>
        public TokenSwapProgram(PublicKey programId) : base(programId, TokenSwapProgramName) { }

        /// <summary>
        /// Create the authority
        /// </summary>
        /// <returns>The swap authority</returns>
        /// <exception cref="InvalidProgramException">No program account could be found (exhausted nonces)</exception>
        public virtual (PublicKey pubkey, byte nonce) CreateAuthority(PublicKey tokenSwapAccount)
        {
            if (!AddressExtensions.TryFindProgramAddress(new[] { tokenSwapAccount.KeyBytes }, ProgramIdKey.KeyBytes, out var addressBytes, out var nonce))
                throw new InvalidProgramException();
            var auth = new PublicKey(addressBytes);
            return (auth, (byte)nonce);
        }

        /// <summary>
        /// Initializes a new swap.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to initialize.</param>
        /// <param name="tokenAAccount">token_a Account. Must be non zero, owned by swap authority.</param>
        /// <param name="tokenBAccount">token_b Account. Must be non zero, owned by swap authority.</param>
        /// <param name="poolTokenMint">Pool Token Mint. Must be empty, owned by swap authority.</param>
        /// <param name="poolTokenFeeAccount">Pool Token Account to deposit trading and withdraw fees. Must be empty, not owned by swap authority.</param>
        /// <param name="userPoolTokenAccount">Pool Token Account to deposit the initial pool token supply.  Must be empty, not owned by swap authority.</param>
        /// <param name="fees">Fees to use for this token swap.</param>
        /// <param name="swapCurve">Curve to use for this token swap.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction Initialize(
            PublicKey tokenSwapAccount,
            PublicKey tokenAAccount,
            PublicKey tokenBAccount,
            PublicKey poolTokenMint,
            PublicKey poolTokenFeeAccount,
            PublicKey userPoolTokenAccount,
            Fees fees, SwapCurve swapCurve)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(tokenSwapAccount, true),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(tokenAAccount, false),
                AccountMeta.ReadOnly(tokenBAccount, false),
                AccountMeta.Writable(poolTokenMint, false),
                AccountMeta.ReadOnly(poolTokenFeeAccount, false),
                AccountMeta.Writable(userPoolTokenAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeInitializeData(nonce, fees, swapCurve)
            };
        }

        /// <summary>
        /// Swap the tokens in the pool.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to operate over.</param>
        /// <param name="userTransferAuthority">user transfer authority.</param>
        /// <param name="tokenSourceAccount">token_(A|B) SOURCE Account, amount is transferable by user transfer authority.</param>
        /// <param name="tokenBaseIntoAccount">token_(A|B) Base Account to swap INTO.  Must be the SOURCE token.</param>
        /// <param name="tokenBaseFromAccount">token_(A|B) Base Account to swap FROM.  Must be the DESTINATION token.</param>
        /// <param name="tokenDestinationAccount">token_(A|B) DESTINATION Account assigned to USER as the owner.</param>
        /// <param name="poolTokenMint">Pool token mint, to generate trading fees.</param>
        /// <param name="poolTokenFeeAccount">Fee account, to receive trading fees.</param>
        /// <param name="poolTokenHostFeeAccount">Host fee account to receive additional trading fees.</param>
        /// <param name="amountIn">SOURCE amount to transfer, output to DESTINATION is based on the exchange rate.</param>
        /// <param name="amountOut">Minimum amount of DESTINATION token to output, prevents excessive slippage.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction Swap(
            PublicKey tokenSwapAccount,
            PublicKey userTransferAuthority,
            PublicKey tokenSourceAccount,
            PublicKey tokenBaseIntoAccount,
            PublicKey tokenBaseFromAccount,
            PublicKey tokenDestinationAccount,
            PublicKey poolTokenMint,
            PublicKey poolTokenFeeAccount,
            PublicKey poolTokenHostFeeAccount,
            ulong amountIn, ulong amountOut)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(tokenSwapAccount, false),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, false),
                AccountMeta.Writable(tokenSourceAccount, false),
                AccountMeta.Writable(tokenBaseIntoAccount, false),
                AccountMeta.Writable(tokenBaseFromAccount, false),
                AccountMeta.Writable(tokenDestinationAccount, false),
                AccountMeta.Writable(poolTokenMint, false),
                AccountMeta.Writable(poolTokenFeeAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            if (poolTokenHostFeeAccount != null)
            {
                keys.Add(AccountMeta.Writable(poolTokenHostFeeAccount, false));
            }
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeSwapData(amountIn, amountOut)
            };
        }

        /// <summary>
        /// Deposit both types of tokens into the pool.  The output is a "pool"
        ///   token representing ownership in the pool. Inputs are converted to
        ///   the current ratio.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to operate over.</param>
        /// <param name="userTransferAuthority">user transfer authority.</param>
        /// <param name="tokenAuserAccount">token_a - user transfer authority can transfer amount.</param>
        /// <param name="tokenBuserAccount">token_b - user transfer authority can transfer amount.</param>
        /// <param name="tokenADepositAccount">token_a Base Account to deposit into.</param>
        /// <param name="tokenBDepositAccount">token_b Base Account to deposit into.</param>
        /// <param name="poolTokenMint">Pool MINT account, swap authority is the owner.</param>
        /// <param name="poolTokenUserAccount">Pool Account to deposit the generated tokens, user is the owner.</param>
        /// <param name="poolTokenAmount">Pool token amount to transfer. token_a and token_b amount are set by the current exchange rate and size of the pool.</param>
        /// <param name="maxTokenA">Maximum token A amount to deposit, prevents excessive slippage.</param>
        /// <param name="maxTokenB">Maximum token B amount to deposit, prevents excessive slippage.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction DepositAllTokenTypes(
            PublicKey tokenSwapAccount,
            PublicKey userTransferAuthority,
            PublicKey tokenAuserAccount,
            PublicKey tokenBuserAccount,
            PublicKey tokenADepositAccount,
            PublicKey tokenBDepositAccount,
            PublicKey poolTokenMint,
            PublicKey poolTokenUserAccount,
            ulong poolTokenAmount, ulong maxTokenA, ulong maxTokenB)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(tokenSwapAccount, false),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, false),
                AccountMeta.Writable(tokenAuserAccount, false),
                AccountMeta.Writable(tokenBuserAccount, false),
                AccountMeta.Writable(tokenADepositAccount, false),
                AccountMeta.Writable(tokenBDepositAccount, false),
                AccountMeta.Writable(poolTokenMint, false),
                AccountMeta.Writable(poolTokenUserAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeDepositAllTokenTypesData(poolTokenAmount, maxTokenA, maxTokenB)
            };
        }

        /// <summary>
        /// Withdraw both types of tokens from the pool at the current ratio, given
        ///   pool tokens.  The pool tokens are burned in exchange for an equivalent
        ///   amount of token A and B.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to operate over.</param>
        /// <param name="userTransferAuthority">user transfer authority.</param>
        /// <param name="poolTokenMint">Pool MINT account, swap authority is the owner.</param>
        /// <param name="sourcePoolAccount">SOURCE Pool account, amount is transferable by user transfer authority.</param>
        /// <param name="tokenASwapAccount">token_a Swap Account to withdraw FROM.</param>
        /// <param name="tokenBSwapAccount">token_b Swap Account to withdraw FROM.</param>
        /// <param name="tokenAUserAccount">token_a user Account to credit.</param>
        /// <param name="tokenBUserAccount">token_b user Account to credit.</param>
        /// <param name="feeAccount">Fee account, to receive withdrawal fees.</param>
        /// <param name="poolTokenAmount">Amount of pool tokens to burn. User receives an output of token a and b based on the percentage of the pool tokens that are returned.</param>
        /// <param name="minTokenA">Minimum amount of token A to receive, prevents excessive slippage.</param>
        /// <param name="minTokenB">Minimum amount of token B to receive, prevents excessive slippage.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction WithdrawAllTokenTypes(
            PublicKey tokenSwapAccount,
            PublicKey userTransferAuthority,
            PublicKey poolTokenMint,
            PublicKey sourcePoolAccount,
            PublicKey tokenASwapAccount,
            PublicKey tokenBSwapAccount,
            PublicKey tokenAUserAccount,
            PublicKey tokenBUserAccount,
            PublicKey feeAccount,
            ulong poolTokenAmount, ulong minTokenA, ulong minTokenB)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(tokenSwapAccount, false),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, false),
                AccountMeta.Writable(poolTokenMint, false),
                AccountMeta.Writable(sourcePoolAccount, false),
                AccountMeta.Writable(tokenASwapAccount, false),
                AccountMeta.Writable(tokenBSwapAccount, false),
                AccountMeta.Writable(tokenAUserAccount, false),
                AccountMeta.Writable(tokenBUserAccount, false),
                AccountMeta.Writable(feeAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeWithdrawAllTokenTypesData(poolTokenAmount, minTokenA, minTokenB)
            };
        }

        /// <summary>
        /// Deposit one type of tokens into the pool.  The output is a "pool" token
        ///   representing ownership into the pool. Input token is converted as if
        ///   a swap and deposit all token types were performed.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to operate over.</param>
        /// <param name="userTransferAuthority">user transfer authority.</param>
        /// <param name="sourceAccount">token_(A|B) SOURCE Account, amount is transferable by user transfer authority.</param>
        /// <param name="destinationTokenAAccount">token_a Swap Account, may deposit INTO.</param>
        /// <param name="destinationTokenBAccount">token_b Swap Account, may deposit INTO.</param>
        /// <param name="poolMintAccount">Pool MINT account, swap authority is the owner.</param>
        /// <param name="poolTokenUserAccount">Pool Account to deposit the generated tokens, user is the owner.</param>
        /// <param name="sourceTokenAmount">Token amount to deposit.</param>
        /// <param name="minPoolTokenAmount">Pool token amount to receive in exchange. The amount is set by the current exchange rate and size of the pool.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction DepositSingleTokenTypeExactAmountIn(
            PublicKey tokenSwapAccount,
            PublicKey userTransferAuthority,
            PublicKey sourceAccount,
            PublicKey destinationTokenAAccount,
            PublicKey destinationTokenBAccount,
            PublicKey poolMintAccount,
            PublicKey poolTokenUserAccount,
            ulong sourceTokenAmount, ulong minPoolTokenAmount)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(tokenSwapAccount, false),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, false),
                AccountMeta.Writable(sourceAccount, false),
                AccountMeta.Writable(destinationTokenAAccount, false),
                AccountMeta.Writable(destinationTokenBAccount, false),
                AccountMeta.Writable(poolMintAccount, false),
                AccountMeta.Writable(poolTokenUserAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeDepositSingleTokenTypeExactAmountInData(sourceTokenAmount, minPoolTokenAmount)
            };
        }

        /// <summary>
        /// Withdraw one token type from the pool at the current ratio given the
        ///   exact amount out expected.
        /// </summary>
        /// <param name="tokenSwapAccount">The token swap account to operate over.</param>
        /// <param name="userTransferAuthority">user transfer authority.</param>
        /// <param name="poolMintAccount">Pool mint account, swap authority is the owner.</param>
        /// <param name="sourceUserAccount">SOURCE Pool account, amount is transferable by user transfer authority.</param>
        /// <param name="tokenASwapAccount">token_a Swap Account to potentially withdraw from.</param>
        /// <param name="tokenBSwapAccount">token_b Swap Account to potentially withdraw from.</param>
        /// <param name="tokenUserAccount">token_(A|B) User Account to credit.</param>
        /// <param name="feeAccount">Fee account, to receive withdrawal fees.</param>
        /// <param name="destTokenAmount">Amount of token A or B to receive.</param>
        /// <param name="maxPoolTokenAmount">Maximum amount of pool tokens to burn. User receives an output of token A or B based on the percentage of the pool tokens that are returned.</param>
        /// <returns>The transaction instruction.</returns>
        public virtual TransactionInstruction WithdrawSingleTokenTypeExactAmountOut(
            PublicKey tokenSwapAccount,
            PublicKey userTransferAuthority,
            PublicKey poolMintAccount,
            PublicKey sourceUserAccount,
            PublicKey tokenASwapAccount,
            PublicKey tokenBSwapAccount,
            PublicKey tokenUserAccount,
            PublicKey feeAccount,
            ulong destTokenAmount, ulong maxPoolTokenAmount)
        {
            var (swapAuthority, nonce) = CreateAuthority(tokenSwapAccount);
            List<AccountMeta> keys = new()
            {
                AccountMeta.ReadOnly(tokenSwapAccount, false),
                AccountMeta.ReadOnly(swapAuthority, false),
                AccountMeta.ReadOnly(userTransferAuthority, false),
                AccountMeta.Writable(poolMintAccount, false),
                AccountMeta.Writable(sourceUserAccount, false),
                AccountMeta.Writable(tokenASwapAccount, false),
                AccountMeta.Writable(tokenBSwapAccount, false),
                AccountMeta.Writable(tokenUserAccount, false),
                AccountMeta.Writable(feeAccount, false),
                AccountMeta.ReadOnly(TokenProgram.ProgramIdKey, false),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenSwapProgramData.EncodeWithdrawSingleTokenTypeExactAmountOutData(destTokenAmount, maxPoolTokenAmount)
            };
        }

        /// <summary>
        /// Decodes an instruction created by the System Program.
        /// </summary>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        /// <returns>A decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            uint instruction = data.GetU8(TokenProgramData.MethodOffset);
            TokenSwapProgramInstructions.Values instructionValue =
                (TokenSwapProgramInstructions.Values)Enum.Parse(typeof(TokenSwapProgramInstructions.Values), instruction.ToString());

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = TokenSwapProgram.TokenSwapProgramIdKey,
                InstructionName = TokenSwapProgramInstructions.Names[instructionValue],
                ProgramName = TokenSwapProgram.TokenSwapProgramName,
                Values = new Dictionary<string, object>(),
                InnerInstructions = new List<DecodedInstruction>()
            };

            switch (instructionValue)
            {
                case TokenSwapProgramInstructions.Values.Initialize:
                    TokenSwapProgramData.DecodeInitializeData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenSwapProgramInstructions.Values.Swap:
                    TokenSwapProgramData.DecodeSwapData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenSwapProgramInstructions.Values.DepositAllTokenTypes:
                    TokenSwapProgramData.DecodeDepositAllTokenTypesData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenSwapProgramInstructions.Values.WithdrawAllTokenTypes:
                    TokenSwapProgramData.DecodeWithdrawAllTokenTypesData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenSwapProgramInstructions.Values.DepositSingleTokenTypeExactAmountIn:
                    TokenSwapProgramData.DecodeDepositSingleTokenTypeExactAmountInData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenSwapProgramInstructions.Values.WithdrawSingleTokenTypeExactAmountOut:
                    TokenSwapProgramData.DecodeWithdrawSingleTokenTypeExactAmountOutData(decodedInstruction, data, keys, keyIndices);
                    break;
            }
            return decodedInstruction;
        }
    }
}