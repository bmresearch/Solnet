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
        /// Mint account account layout size.
        /// </summary>
        public const int MintAccountDataSize = 82;
        
        /// <summary>
        /// Account layout size.
        /// </summary>
        public const int TokenAccountDataSize = 165;

        /// <summary>
        /// Initializes an instruction to transfer tokens from one account to another either directly or via a delegate.
        /// If this account is associated with the native mint then equal amounts of SOL and Tokens will be transferred to the destination account.
        /// </summary>
        /// <param name="source">The public key of the account to transfer tokens from.</param>
        /// <param name="destination">The public key of the account to account to transfer tokens to.</param>
        /// <param name="amount">The amount of tokens to transfer.</param>
        /// <param name="owner">The public key of the owner account.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Transfer(
            PublicKey source, PublicKey destination, ulong amount, PublicKey owner)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(source, false),
                AccountMeta.Writable(destination, false),
                AccountMeta.ReadOnly(owner, true)
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
        /// <param name="owner">The public key of the owner account.</param>
        /// <param name="tokenMint">The public key of the token mint.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction TransferChecked(
            PublicKey source, PublicKey destination, ulong amount, int decimals, PublicKey owner, PublicKey tokenMint, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(source, true),
                AccountMeta.ReadOnly(tokenMint, false),
                AccountMeta.Writable(destination, false),
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
        /// as the system program's <see cref="SystemProgram.CreateAccount(PublicKey,PublicKey,ulong,ulong,PublicKey)"/>"/>
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
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(owner, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false)
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
                AccountMeta.Writable(multiSignature, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false)
            };
            keys.AddRange(signers.Select(signer => AccountMeta.ReadOnly(signer, false)));
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
                AccountMeta.Writable(mint, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false)
            };

            int freezeAuthorityOpt = freezeAuthority != null ? 1 : 0;
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMintData(
                    mintAuthority,
                    freezeAuthority ?? new Account().PublicKey,
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
        public static TransactionInstruction MintTo(PublicKey mint, PublicKey destination, ulong amount, PublicKey mintAuthority)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mint, false),
                AccountMeta.Writable(destination, false),
                AccountMeta.ReadOnly(mintAuthority, true)
            };

            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeMintToData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to approve a transaction.
        /// </summary>
        /// <param name="source">The public key source account.</param>
        /// <param name="delegatePublicKey">The public key of the delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Approve(
            PublicKey source, PublicKey delegatePublicKey, PublicKey owner, ulong amount, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(source, false),
                AccountMeta.ReadOnly(delegatePublicKey, false)
            };

            keys = AddSigners(keys, owner, signers);

            return new TransactionInstruction { ProgramId = ProgramIdKey.KeyBytes, Keys = keys, Data = TokenProgramData.EncodeApproveData(amount) };
        }

        /// <summary>
        /// Initializes an instruction to revoke a transaction.
        /// </summary>
        /// <param name="delegatePublicKey">The public key of the delegate account authorized to perform a transfer from the source account.</param>
        /// <param name="ownerAccount">The public key of the owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Revoke(PublicKey delegatePublicKey, PublicKey ownerAccount,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new ()
            {
                AccountMeta.ReadOnly(delegatePublicKey, false),
            };
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
        /// <param name="account">The public key of the account to set the authority on.</param>
        /// <param name="authority">The type of authority to set.</param>
        /// <param name="currentAuthority">The public key of the current authority of the specified type.</param>
        /// <param name="newAuthority">The public key of the new authority.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction SetAuthority(
            PublicKey account, AuthorityType authority, PublicKey currentAuthority, PublicKey newAuthority = null, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(SystemProgram.SysVarRentKey, false)
            };
            keys = AddSigners(keys, currentAuthority, signers);

            int newAuthorityOpt = newAuthority != null ? 1 : 0;
            newAuthority ??= new Account().PublicKey;
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeSetAuthorityData(authority, newAuthorityOpt, newAuthority)
            };
        }

        /// <summary>
        /// Initialize an instruction to burn tokens.
        /// </summary>
        /// <param name="account">The public key of the account to burn tokens from.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <param name="amount">The amount of tokens to burn.</param>
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction Burn(PublicKey account, PublicKey mint, ulong amount, PublicKey owner, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
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
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction CloseAccount(PublicKey account, PublicKey destination, PublicKey owner, PublicKey programId, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.Writable(destination, false),
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
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction FreezeAccount(PublicKey account, PublicKey mint, PublicKey owner, PublicKey programId, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
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
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="programId">The public key which represents the associated program id.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ThawAccount(PublicKey account, PublicKey mint, PublicKey owner, PublicKey programId, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
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
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="amount">The maximum amount of tokens the delegate may transfer.</param>
        /// <param name="signers">Signing accounts if the `owner` is a multi signature.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="mint">The public key of the token mint.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction ApproveChecked(
            PublicKey source, PublicKey delegatePublicKey, ulong amount, byte decimals, PublicKey owner, PublicKey mint, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(source, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(delegatePublicKey, false),
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
        /// <param name="mintAuthority">The public key of the token mint authority account.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction MintToChecked(PublicKey mint, PublicKey destination, PublicKey mintAuthority, ulong amount, int decimals, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(mint, false),
                AccountMeta.Writable(destination, false),
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
        /// <param name="owner">The public key of the owner account of the source account.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The token decimals.</param>
        /// <param name="signers">Signing accounts if the <c>account</c> is a multi signature.</param>
        /// <returns>The transaction instruction.</returns>
        public static TransactionInstruction BurnChecked(PublicKey mint, PublicKey account, PublicKey owner, ulong amount, int decimals, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys = new()
            {
                AccountMeta.Writable(account, false),
                AccountMeta.Writable(mint, false),
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
        /// <param name="owner">The public key of the owner account.</param>
        /// <param name="signers">The list of signers.</param>
        /// <returns>The list of keys with the added signers.</returns>
        private static List<AccountMeta> AddSigners(List<AccountMeta> keys, PublicKey owner = null,
            IEnumerable<PublicKey> signers = null)
        {
            if (signers != null)
            {
                keys.Add(AccountMeta.ReadOnly(owner,  true));
                keys.AddRange(signers.Select(signer => AccountMeta.ReadOnly(signer, true)));
            }
            else
            {
                keys.Add(AccountMeta.ReadOnly(owner, true));
            }
            return keys;
        }
    }
}