using System.Collections.Generic;
using NBitcoin.DataEncoders;
using Solnet.Rpc.Models;

namespace Solnet.Programs
{
    public static class TokenProgram
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new ();

        /// <summary>
        /// The public key of the sysvar rent.
        /// </summary>
        private static string SysvarRentPublicKey = "SysvarRent111111111111111111111111111111111";

        private static int TransferMethodId = 3;

        private static int TransferMethodCheckedId = 12;
        
        /// <summary>
        /// The address of the Token Program.
        /// </summary>
        public static string ProgramId = "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA";

        /// <summary>
        /// Transfers tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="amount"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static TransactionInstruction Transfer(
            string source, string destination, long amount, string owner)
        {
            var keys = new List<AccountMeta>
            {
                new (Encoder.DecodeData(source), false, true),
                new (Encoder.DecodeData(destination), false, true),
                new (Encoder.DecodeData(owner), true, false)
            };


            var transactionData = EncodeTransferData(amount);

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = transactionData
            };
        }

        /// <summary>
        /// <para>
        /// Transfers tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </para>
        /// <para>
        /// This instruction differs from Transfer in that the token mint and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="amount"></param>
        /// <param name="decimals"></param>
        /// <param name="owner"></param>
        /// <param name="tokenMint"></param>
        /// <returns></returns>
        public static TransactionInstruction TransferChecked(
            string source, string destination, long amount, byte decimals, string owner, string tokenMint) 
        {
            var keys = new List<AccountMeta>
            {
                new (Encoder.DecodeData(source), false, true),
                new (Encoder.DecodeData(tokenMint), false, false),
                new (Encoder.DecodeData(destination), false, true),
                new (Encoder.DecodeData(owner), true, false)
            };

            var transactionData = EncodeTransferCheckedData(amount, decimals);

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = transactionData
            };
        }

        /// <summary>
        /// <para>Initializes a new account to hold tokens.
        /// If this account is associated with the native mint then the token balance of the initialized account will be equal to the amount of SOL in the account.
        /// If this account is associated with another mint, that mint must be initialized before this command can succeed.
        /// </para>
        /// <para>
        /// The InitializeAccount instruction requires no signers and MUST be included within the same Transaction
        /// as the system program's <see cref="SystemProgram.CreateAccount(string, string, long, long, string)"/> or <see cref="SystemProgram.CreateAccount(byte[], byte[], long, long, byte[])"/>
        /// instruction that creates the account being initialized.
        /// Otherwise another party can acquire ownership of the uninitialized account.
        /// </para>
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mint"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static TransactionInstruction InitializeAccount(string account, string mint, string owner) 
        {
            var keys = new List<AccountMeta>();
            byte[] data = new byte[0];

            keys.Add(new AccountMeta(Encoder.DecodeData(account),false, true));
            keys.Add(new AccountMeta(Encoder.DecodeData(mint), false, false));
            keys.Add(new AccountMeta(Encoder.DecodeData(owner),false, true));
            keys.Add(new AccountMeta(Encoder.DecodeData(SysvarRentPublicKey),false, false));

            return new()
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = data
            };
        }
        
        /// <summary>
        /// Initialize a mint transaction.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mint"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static TransactionInstruction InitializeMint(string mint, string mintAuthority, string owner) 
        {
            var keys = new List<AccountMeta>();
            byte[] data = new byte[0];

            keys.Add(new AccountMeta(Encoder.DecodeData(mint),false, true));
            keys.Add(new AccountMeta(Encoder.DecodeData(SysvarRentPublicKey),false, false));

            return new()
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = data
            };
        }
        
        /// <summary>
        /// Encode the transaction instruction data for the <c>Transfer</c> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeInitializeMintData(
            byte[] mintAuthority, byte[] freezeAuthority, int decimals, int freezeAuthorityOption)
        {
            var methodBuffer = new byte[66];
            
            Utils.Uint32ToByteArrayLe(TransferMethodId, methodBuffer, 0);
            Utils.Int64ToByteArrayLe(decimals, methodBuffer, 4);

            return methodBuffer;
        }
        
        
        /// <summary>
        /// Encode the transaction instruction data for the <c>Transfer</c> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeTransferData(long amount)
        {
            var methodBuffer = new byte[12];
            
            Utils.Uint32ToByteArrayLe(TransferMethodId, methodBuffer, 0);
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 4);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <c>TransferChecked</c> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The number of decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeTransferCheckedData(long amount, byte decimals) 
        {
            var methodBuffer = new byte[13];

            Utils.Uint32ToByteArrayLe(TransferMethodCheckedId, methodBuffer, 0);
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 4);
            methodBuffer[12] = decimals;

            return methodBuffer;
        }
    }
}