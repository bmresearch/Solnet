using NBitcoin.DataEncoders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    public static class TokenProgram
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new();

        /// <summary>
        /// The address of the Token Program.
        /// </summary>
        public const string ProgramId = "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA";

        /// <summary>
        /// The public key of the sysvar rent.
        /// </summary>
        public static string SysvarRentPublicKey = "SysvarRent111111111111111111111111111111111";

        /// <summary>
        /// Mint account ccount layout size.
        /// </summary>
        public const int MintAccountDataSize = 82;

        /// <summary>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="source">The account to transfer tokens from.</param>
        /// <param name="destination">The account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="owner">The account owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(
            string source, string destination, long amount, string owner)
        {
            return Transfer(Encoder.DecodeData(source), Encoder.DecodeData(destination), amount,
                Encoder.DecodeData(owner));
        }

        /// <summary>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="source">The account to transfer tokens from.</param>
        /// <param name="destination">The account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="owner">The account owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(
            byte[] source, byte[] destination, long amount, byte[] owner)
        {
            var keys = new List<AccountMeta>
            {
                new(source, false, true), new(destination, false, true), new(owner, true, false)
            };

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = EncodeTransferData(amount)
            };
        }

        /// <summary>
        /// <para>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </para>
        /// <para>
        /// This instruction differs from Transfer in that the token mint and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="source">The account to transfer tokens from.</param>
        /// <param name="destination">The account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="owner">The account owner.</param>
        /// <param name="tokenMint">The token mint.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferChecked(
            string source, string destination, long amount, byte decimals, string owner, string tokenMint)
        {
            return TransferChecked(
                Encoder.DecodeData(source),
                Encoder.DecodeData(destination),
                amount,
                decimals,
                Encoder.DecodeData(owner),
                Encoder.DecodeData(tokenMint));
        }


        /// <summary>
        /// <para>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </para>
        /// <para>
        /// This instruction differs from Transfer in that the token mint and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="source">The account to transfer tokens from.</param>
        /// <param name="destination">The account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="owner">The account owner.</param>
        /// <param name="tokenMint">The token mint.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferChecked(
            byte[] source, byte[] destination, long amount, byte decimals, byte[] owner, byte[] tokenMint)
        {
            var keys = new List<AccountMeta>
            {
                new(source, false, true),
                new(tokenMint, false, false),
                new(destination, false, true),
                new(owner, true, false)
            };

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = EncodeTransferCheckedData(amount, decimals)
            };
        }

        /// <summary>
        /// <para>Initializes an instruction to initialize a new account to hold tokens.
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
        /// <param name="account">The account to initialize.</param>
        /// <param name="mint">The token mint.</param>
        /// <param name="owner">The account to set as owner of the initialized account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeAccount(string account, string mint, string owner)
        {
            return InitializeAccount(
                Encoder.DecodeData(account),
                Encoder.DecodeData(mint),
                Encoder.DecodeData(owner));
        }

        /// <summary>
        /// <para>Initializes an instruction to initialize a new account to hold tokens.
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
        /// <param name="account">The account to initialize.</param>
        /// <param name="mint">The token mint.</param>
        /// <param name="owner">The account to set as owner of the initialized account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeAccount(byte[] account, byte[] mint, byte[] owner)
        {
            var keys = new List<AccountMeta>
            {
                new(account, false, true),
                new(mint, false, false),
                new(owner, false, false),
                new(Encoder.DecodeData(SysvarRentPublicKey), false, false)
            };

            return new() { ProgramId = Encoder.DecodeData(ProgramId), Keys = keys, Data = EncodeInitializeAccountData() };
        }

        /// <summary>
        /// Initialize an instruction to initialize a token mint account.
        /// </summary>
        /// <param name="mint">The token mint.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <param name="freezeAuthority">The token freeze authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMint(string mint, int decimals, string mintAuthority,
            string freezeAuthority = "")
        {
            return InitializeMint(
                Encoder.DecodeData(mint),
                decimals,
                Encoder.DecodeData(mintAuthority),
                !string.IsNullOrWhiteSpace(freezeAuthority) ? Encoder.DecodeData(mintAuthority) : null);
        }

        /// <summary>
        /// Initialize an instruction to initialize a token mint account.
        /// </summary>
        /// <param name="mint">The token mint.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <param name="freezeAuthority">The token freeze authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeMint(byte[] mint, int decimals, byte[] mintAuthority,
            byte[] freezeAuthority = null)
        {
            var keys = new List<AccountMeta>
            {
                new(mint, false, true), new(Encoder.DecodeData(SysvarRentPublicKey), false, false)
            };

            var freezeAuthorityOpt = freezeAuthority != null ? 1 : 0;
            freezeAuthority ??= new Account().PublicKey;

            return new()
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = EncodeInitializeMintData(
                    mintAuthority,
                    freezeAuthority,
                    decimals,
                    freezeAuthorityOpt)
            };
        }

        /// <summary>
        /// Initializes an instruction to mint tokens to a destination account.
        /// </summary>
        /// <param name="mint">The token mint.</param>
        /// <param name="destination">The account to mint tokens to.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintTo(string mint, string destination, long amount, string mintAuthority)
        {
            return MintTo(
                Encoder.DecodeData(mint),
                Encoder.DecodeData(destination),
                amount,
                Encoder.DecodeData(mintAuthority));
        }

        /// <summary>
        /// Initializes an instruction to mint tokens to a destination account.
        /// </summary>
        /// <param name="mint">The token mint.</param>
        /// <param name="destination">The account to mint tokens to.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintTo(byte[] mint, byte[] destination, long amount, byte[] mintAuthority)
        {
            var keys = new List<AccountMeta>
            {
                new(mint, false, true), new(destination, false, true), new(mintAuthority, true, false)
            };

            return new() { ProgramId = Encoder.DecodeData(ProgramId), Keys = keys, Data = EncodeMintToData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to approve a transaction.
        /// </summary>
        /// <param name="sourceAccount">The source account.</param>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Approve(
            string sourceAccount, string delegateAccount, string ownerAccount, long amount, List<string> signers = null)
        {
            if (signers == null)
            {
                return Approve(
                    Encoder.DecodeData(sourceAccount),
                    Encoder.DecodeData(delegateAccount),
                    Encoder.DecodeData(ownerAccount),
                    amount);
            }

            List<byte[]> byteSigners = new(signers.Count);
            byteSigners.AddRange(signers.Select(signer => Encoder.DecodeData(signer)));
            return Approve(
                Encoder.DecodeData(sourceAccount),
                Encoder.DecodeData(delegateAccount),
                Encoder.DecodeData(ownerAccount),
                amount, byteSigners);
        }

        /// <summary>
        /// Initializes an instruction to approve a transaction.
        /// </summary>
        /// <param name="sourceAccount">The source account.</param>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Approve(
            byte[] sourceAccount, byte[] delegateAccount, byte[] ownerAccount, long amount, List<byte[]> signers = null)
        {
            var keys = new List<AccountMeta> { new(sourceAccount, false, true), new(delegateAccount, false, false) };

            keys = AddSigners(keys, ownerAccount, signers);

            return new() { ProgramId = Encoder.DecodeData(ProgramId), Keys = keys, Data = EncodeApproveData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to revoke a transaction.
        /// </summary>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Revoke(string delegateAccount, string ownerAccount,
            List<string> signers = null)
        {
            if (signers == null)
            {
                return Revoke(
                    Encoder.DecodeData(delegateAccount),
                    Encoder.DecodeData(ownerAccount));
            }

            List<byte[]> byteSigners = new(signers.Count);
            byteSigners.AddRange(signers.Select(signer => Encoder.DecodeData(signer)));
            return Revoke(
                Encoder.DecodeData(delegateAccount),
                Encoder.DecodeData(ownerAccount),
                byteSigners);
        }

        /// <summary>
        /// Initializes an instruction to revoke a transaction.
        /// </summary>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Revoke(byte[] delegateAccount, byte[] ownerAccount,
            List<byte[]> signers = null)
        {
            var keys = new List<AccountMeta> { new(delegateAccount, false, false), };
            keys = AddSigners(keys, ownerAccount, signers);

            return new TransactionInstruction
            {
                ProgramId = Encoder.DecodeData(ProgramId),
                Keys = keys,
                Data = EncodeRevokeData()
            };
        }

        /// <summary>
        /// Adds the list of signers to the list of keys.
        /// </summary>
        /// <param name="keys">The instruction's list of keys.</param>
        /// <param name="owner">The owner account.</param>
        /// <param name="signers">The list of signers.</param>
        /// <returns>The list of keys with the added signers.</returns>
        private static List<AccountMeta> AddSigners(List<AccountMeta> keys, byte[] owner = null,
            IEnumerable<byte[]> signers = null)
        {
            if (signers != null)
            {
                keys.Add(new AccountMeta(owner, false, false));
                keys.AddRange(signers.Select(signer => new AccountMeta(signer, true, false)));
            }
            else
            {
                keys.Add(new AccountMeta(owner, true, false));
            }

            return keys;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Revoke"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeRevokeData()
        {
            var methodBuffer = new byte[1];
            methodBuffer[0] = (byte)TokenProgramInstructions.Revoke;
            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Approve"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens to approve the transfer of.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeApproveData(long amount)
        {
            var methodBuffer = new byte[9];

            methodBuffer[0] = (byte)TokenProgramInstructions.Approve;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);
            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.InitializeAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeInitializeAccountData() => new[] { (byte)TokenProgramInstructions.InitializeAccount };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.InitializeMint"/> method.
        /// </summary>
        /// <param name="mintAuthority">The mint authority for the token.</param>
        /// <param name="freezeAuthority">The freeze authority for the token.</param>
        /// <param name="decimals">The amount of decimals.</param>
        /// <param name="freezeAuthorityOption">The freeze authority option for the token.</param>
        /// <remarks>The <c>freezeAuthorityOption</c> parameter is related to the existence or not of a freeze authority.</remarks>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeInitializeMintData(
            byte[] mintAuthority, byte[] freezeAuthority, int decimals, int freezeAuthorityOption)
        {
            var methodBuffer = new byte[67];

            methodBuffer[0] = (byte)TokenProgramInstructions.InitializeMint;
            methodBuffer[1] = (byte)decimals;
            Array.Copy(mintAuthority, 0, methodBuffer, 2, 32);
            methodBuffer[34] = (byte)freezeAuthorityOption;
            Array.Copy(freezeAuthority, 0, methodBuffer, 35, 32);

            return methodBuffer;
        }


        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Transfer"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeTransferData(long amount)
        {
            var methodBuffer = new byte[9];

            methodBuffer[0] = (byte)TokenProgramInstructions.Transfer;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.TransferChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The number of decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeTransferCheckedData(long amount, byte decimals)
        {
            var methodBuffer = new byte[10];

            methodBuffer[0] = (byte)TokenProgramInstructions.TransferChecked;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);
            methodBuffer[9] = decimals;

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.MintTo"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeMintToData(long amount)
        {
            var methodBuffer = new byte[9];

            methodBuffer[0] = (byte)TokenProgramInstructions.MintTo;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);

            return methodBuffer;
        }
    }
}