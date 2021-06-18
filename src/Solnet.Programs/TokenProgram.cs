using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Token Program methods.
    /// <remarks>
    /// For more information see: https://spl.solana.com/token
    /// </remarks>
    /// </summary>
    public static class TokenProgram
    {
        /// <summary>
        /// The base58 encoder instance.
        /// </summary>
        private static readonly Base58Encoder Encoder = new();

        /// <summary>
        /// The public key of the Token Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

        /// <summary>
        /// The public key of the sysvar rent.
        /// </summary>
        public static readonly PublicKey SysvarRentKey = new ("SysvarRent111111111111111111111111111111111");

        /// <summary>
        /// Mint account account layout size.
        /// </summary>
        public const int MintAccountDataSize = 82;

        /// <summary>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="source">The public key of the account to transfer tokens from.</param>
        /// <param name="destination">The public key of the account to account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="owner">The account owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(
            PublicKey source, PublicKey destination, long amount, Account owner)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(source, true), new AccountMeta(destination, true), new AccountMeta(owner, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeTransferData(amount)
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
        /// <param name="source">The public key of the account to transfer tokens from.</param>
        /// <param name="destination">The public key of the account to account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="owner">The account owner.</param>
        /// <param name="tokenMint">The public key token mint.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferChecked(
            PublicKey source, PublicKey destination, long amount, byte decimals, Account owner, PublicKey tokenMint)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(source, true),
                new AccountMeta(tokenMint, false),
                new AccountMeta(destination, true),
                new AccountMeta(owner, false)
            };

            return new()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeTransferCheckedData(amount, decimals)
            };
        }
        
        /// <summary>
        /// <para>Initializes an instruction to initialize a new account to hold tokens.
        /// If this account is associated with the native mint then the token balance of the initialized account will be equal to the amount of SOL in the account.
        /// If this account is associated with another mint, that mint must be initialized before this command can succeed.
        /// </para>
        /// <para>
        /// The InitializeAccount instruction requires no signers and MUST be included within the same Transaction
        /// as the system program's <see cref="SystemProgram.CreateAccount(Account,PublicKey,long,long,PublicKey)"/>"/>
        /// instruction that creates the account being initialized.
        /// Otherwise another party can acquire ownership of the uninitialized account.
        /// </para>
        /// </summary>
        /// <param name="account">The public key of the account to initialize.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="owner">The public key of the account to set as owner of the initialized account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction InitializeAccount(PublicKey account, PublicKey mint, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(mint, false),
                new AccountMeta(owner, false),
                new AccountMeta(SysvarRentKey, false)
            };

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys, 
                Data = TokenProgramData.EncodeInitializeAccountData()
            };
        }

        /// <summary>
        /// Initializes an instruction to initialize a multi signature token account.
        /// </summary>
        /// <param name="multiSignature">Public key of the multi signature account.</param>
        /// <param name="signers">Addresses of multi signature signers.</param>
        /// <param name="m">The number of signatures required to validate this multi signature account.</param>
        public static TransactionInstruction InitializeMultiSignature(PublicKey multiSignature, List<PublicKey> signers, int m)
        {
            var keys = new List<AccountMeta>()
            {
                new(multiSignature, true),
                new(SysvarRentKey, false)
            };
            keys.AddRange(signers.Select(signer => new AccountMeta(signer, false)));

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes, 
                Keys = keys, 
                Data = TokenProgramData.EncodeInitializeMultiSignatureData(m)
            };
        }

        /// <summary>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="mint">The public keytoken mint.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <param name="freezeAuthority">The token freeze authority.</param>
        public static TransactionInstruction InitializeMint(PublicKey mint, int decimals, PublicKey mintAuthority,
            PublicKey freezeAuthority = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(mint, true), new AccountMeta(SysvarRentKey, false)
            };

            int freezeAuthorityOpt = freezeAuthority != null ? 1 : 0;
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMintData(
                    mintAuthority.KeyBytes,
                    freezeAuthority != null ? freezeAuthority.KeyBytes : new Account().PublicKey.KeyBytes,
                    decimals,
                    freezeAuthorityOpt)
            };
        }

        /// <param name="source">The account to transfer tokens from.</param>
        /// <param name="destination">The account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="owner">The account owner.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(
            string source, string destination, long amount, string owner)
        {
            return new();
        }


        /// <summary>
        /// Initializes an instruction to mint tokens to a destination account.
        /// </summary>
        /// <param name="mint">The token mint.</param>
        /// <param name="destination">The account to mint tokens to.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="mintAuthority">The token mint authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintTo(PublicKey mint, PublicKey destination, long amount, Account mintAuthority)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(mint, true),
                new AccountMeta(destination, true),
                new AccountMeta(mintAuthority, false)
            };

            return new TransactionInstruction() { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeMintToData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to approve a transaction.
        /// </summary>
        /// <param name="sourceAccount">The source account.</param>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Approve(
            PublicKey sourceAccount, PublicKey delegateAccount, Account ownerAccount, long amount, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new() { new AccountMeta(sourceAccount, true), new AccountMeta(delegateAccount, false) };

            keys = AddSigners(keys, ownerAccount, signers);

            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeApproveData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to revoke a transaction.
        /// </summary>
        /// <param name="delegateAccount">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Revoke(PublicKey delegateAccount, Account ownerAccount,
            IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new () { new AccountMeta(delegateAccount, false), };
            keys = AddSigners(keys, ownerAccount, signers);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeRevokeData()
            };
        }
        
        
        /// <summary>
        /// Initialize an instruction to set an authority on an account.
        /// </summary>
        /// <param name="account">The account to set the authority on.</param>
        /// <param name="authority">The type of authority to set.</param>
        /// <param name="currentAuthority">The current authority of the specified type.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <param name="newAuthority">The new authority.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetAuthority(
            string account, AuthorityType authority, string currentAuthority, string newAuthority, List<string> signers = null)
        {
            if (signers == null)
            {
                return SetAuthority(
                    Encoder.DecodeData(account),
                    authority,
                    Encoder.DecodeData(currentAuthority),
                    Encoder.DecodeData(newAuthority));
            }

            List<byte[]> byteSigners = new(signers.Count);
            byteSigners.AddRange(signers.Select(signer => Encoder.DecodeData(signer)));
            return SetAuthority(
                Encoder.DecodeData(account),
                authority,
                Encoder.DecodeData(currentAuthority),
                Encoder.DecodeData(newAuthority),
                byteSigners);
        }
        
        /// <inheritdoc cref="SetAuthority(string,Solnet.Programs.AuthorityType,string,string,System.Collections.Generic.List{string})"/>
        public static TransactionInstruction SetAuthority(
            byte[] account, AuthorityType authority, byte[] currentAuthority, byte[] newAuthority, List<byte[]> signers = null)
        {
            
            
            return new TransactionInstruction() { };
        }
        
        
        public static TransactionInstruction Burn()
        {
            return new TransactionInstruction() { };
        }
        

        public static TransactionInstruction CloseAccount()
        {
            return new TransactionInstruction() { };
        }
        
        public static TransactionInstruction FreezeAccount()
        {
            return new TransactionInstruction() { };
        }
        
        public static TransactionInstruction ThawAccount()
        {
            return new TransactionInstruction() { };
        }
        
        public static TransactionInstruction ApproveChecked()
        {
            return new TransactionInstruction() { };
        }
        
        public static TransactionInstruction MintToChecked()
        {
            return new TransactionInstruction() { };
        }
        
        public static TransactionInstruction BurnChecked()
        {
            return new TransactionInstruction() { };
        }
        
        /// <summary>
        /// Adds the list of signers to the list of keys.
        /// </summary>
        /// <param name="keys">The instruction's list of keys.</param>
        /// <param name="owner">The owner account.</param>
        /// <param name="signers">The list of signers.</param>
        /// <returns>The list of keys with the added signers.</returns>
        private static List<AccountMeta> AddSigners(List<AccountMeta> keys, Account owner = null,
            IEnumerable<Account> signers = null)
        {
            if (signers != null)
            {
                keys.Add(new AccountMeta(owner?.PublicKey,  false));
                keys.AddRange(signers.Select(signer => new AccountMeta(signer, false)));
            }
            else
            {
                keys.Add(new AccountMeta(owner, false));
            }

            return keys;
        }
    }
}