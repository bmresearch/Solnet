using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
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
            PublicKey source, PublicKey destination, ulong amount, Account owner)
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
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferChecked(
            PublicKey source, PublicKey destination, ulong amount, int decimals, Account owner, PublicKey tokenMint, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(source, true),
                new AccountMeta(tokenMint, false),
                new AccountMeta(destination, true),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction
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
        /// as the system program's <see cref="SystemProgram.CreateAccount(Account,Account,ulong,ulong,PublicKey)"/>"/>
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
        public static TransactionInstruction InitializeMultiSignature(PublicKey multiSignature, IEnumerable<PublicKey> signers, int m)
        {
            List<AccountMeta> keys = new ()
            {
                new AccountMeta(multiSignature, true),
                new AccountMeta(SysvarRentKey, false)
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
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mintAuthority">The public key of the token mint authority.</param>
        /// <param name="freezeAuthority">The token freeze authority.</param>
        public static TransactionInstruction InitializeMint(PublicKey mint, int decimals, PublicKey mintAuthority,
            PublicKey freezeAuthority = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(mint, true), new AccountMeta(SysvarRentKey, false)
            };

            int freezeAuthorityOpt = freezeAuthority != null ? 1 : 0;
            return new TransactionInstruction
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

        /// <summary>
        /// Initializes an instruction to mint tokens to a destination account.
        /// </summary>
        /// <param name="mint">The public key token mint.</param>
        /// <param name="destination">The public key of the account to mint tokens to.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="mintAuthority">The token mint authority account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintTo(PublicKey mint, PublicKey destination, ulong amount, Account mintAuthority)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(mint, true),
                new AccountMeta(destination, true),
                new AccountMeta(mintAuthority, false)
            };

            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeMintToData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to approve a transaction.
        /// </summary>
        /// <param name="source">The public key source account.</param>
        /// <param name="delegatePublicKey">The public key of the delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Approve(
            PublicKey source, PublicKey delegatePublicKey, Account owner, ulong amount, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new() { new AccountMeta(source, true), new AccountMeta(delegatePublicKey, false) };

            keys = AddSigners(keys, owner, signers);

            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeApproveData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to revoke a transaction.
        /// </summary>
        /// <param name="delegatePublicKey">The delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multisig.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Revoke(PublicKey delegatePublicKey, Account ownerAccount,
            IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new () { new AccountMeta(delegatePublicKey, false), };
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
        /// <param name="newAuthority">The new authority.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetAuthority(
            PublicKey account, AuthorityType authority, Account currentAuthority, PublicKey newAuthority, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true), new AccountMeta(SysvarRentKey, false)
            };
            keys = AddSigners(keys, currentAuthority, signers);

            int newAuthorityOpt = newAuthority != null ? 1 : 0;
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeSetAuthorityData(authority, newAuthorityOpt, newAuthority.KeyBytes)
            };
        }

        /// <summary>
        /// Initialize an instruction to burn tokens.
        /// </summary>
        /// <param name="account">The public key of the account to burn tokens from.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="amount">The amount of tokens to burn.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Burn(PublicKey account, PublicKey mint, ulong amount, Account owner, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(mint, false),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeBurnData(amount)
            };
        }

        /// <summary>
        /// Initialize an instruction to close an account.
        /// </summary>
        /// <param name="account">The public key of the account to close.</param>
        /// <param name="destination">The public key of the account that will receive the SOL.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CloseAccount(PublicKey account, PublicKey destination, Account owner, PublicKey programId, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(destination, true),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction()
            {
                ProgramId = programId.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeCloseAccountData()
            };
        }
        
        /// <summary>
        /// Initialize an instruction to freeze a token account.
        /// </summary>
        /// <param name="account">The public key of the account to freeze.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FreezeAccount(PublicKey account, PublicKey mint, Account owner, PublicKey programId, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(mint, false),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction()
            {
                ProgramId = programId.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeFreezeAccountData()
            };
        }
        
        /// <summary>
        /// Initialize an instruction to thaw a token account.
        /// </summary>
        /// <param name="account">The public key of the account to thaw.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ThawAccount(PublicKey account, PublicKey mint, Account owner, PublicKey programId, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(mint, false),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction()
            {
                ProgramId = programId.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeThawAccountData()
            };
        }

        /// <summary>
        /// Initialize an instruction to approve a transaction.
        /// <para>
        /// This instruction differs from Approve in that the amount and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="source">The public key of the source account.</param>
        /// <param name="delegatePublicKey">The public key of the delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ApproveChecked(
            PublicKey source, PublicKey delegatePublicKey, ulong amount, byte decimals, Account owner, PublicKey mint, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(source, true),
                new AccountMeta(mint, false),
                new AccountMeta(delegatePublicKey, false),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeApproveCheckedData(amount, decimals)
            };
        }
        
        /// <summary>
        /// Initialize an instruction to approve a transaction.
        /// <para>
        /// This instruction differs from MintTo in that the amount and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="destination">The public key of the account to mint tokens to.</param>
        /// <param name="mintAuthority">The token mint authority account.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintToChecked(PublicKey mint, PublicKey destination, Account mintAuthority, ulong amount, int decimals, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(mint, true),
                new AccountMeta(destination, true),
            };
            keys = AddSigners(keys, mintAuthority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeMintToCheckedData(amount, decimals)
            };
        }

        /// <summary>
        /// Initialize an instruction to burn tokens.
        /// <para>
        /// This instruction differs from Burn in that the amount and decimals value is checked by the caller.
        /// This may be useful when creating transactions offline or within a hardware wallet.
        /// </para>
        /// </summary>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="account">The public key of the account to burn from.</param>
        /// <param name="owner">The owner account of the source account.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction BurnChecked(PublicKey mint, PublicKey account, Account owner, ulong amount, int decimals, IEnumerable<Account> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                new AccountMeta(account, true),
                new AccountMeta(mint, true),
            };
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeBurnCheckedData(amount, decimals)
            };
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