using Solnet.Programs.Utilities;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the Token-2022 Program methods.
    /// </summary>
    public static class Token2022Program
    {
        /// <summary>
        /// The public key of the Token-2022 Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("TokenzQdBNbLqP5VEhdkAS6EPFLC1PHnBqCXEpPxuEb");

        /// <summary>
        /// The public key of the Token-2022 native mint.
        /// </summary>
        public static readonly PublicKey NativeMintProgramIdKey = new("9pan9bMn5HatX4EJdBwg9VgCa7Uz5HL8N1m5D3NdXejP");

        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "Token 2022 Program";

        /// <summary>
        /// Mint account layout size.
        /// </summary>
        public const int MintAccountDataSize = 82;

        /// <summary>
        /// Token account layout size.
        /// </summary>
        public const int TokenAccountDataSize = 165;

        /// <summary>
        /// Multisig account layout size.
        /// </summary>
        public const int MultisigAccountDataSize = 355;

        /// <summary>
        /// Transfers tokens from one account to another.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="amount"></param>
        /// <param name="authority"></param>
        /// <param name="signers"></param>
        /// <returns></returns>
        public static TransactionInstruction Transfer(
            PublicKey source, PublicKey destination, ulong amount, PublicKey authority, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
                AccountMeta.Writable(destination, false),
            ];
            keys = AddSigners(keys, authority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeTransferData(amount)
            };
        }

        /// <summary>
        /// Transfers tokens from one account to another, checking the number of decimals.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="amount"></param>
        /// <param name="decimals"></param>
        /// <param name="authority"></param>
        /// <param name="tokenMint"></param>
        /// <param name="signers"></param>
        /// <returns></returns>
        public static TransactionInstruction TransferChecked(
            PublicKey source, PublicKey destination, ulong amount, int decimals, PublicKey authority, PublicKey tokenMint,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
                AccountMeta.ReadOnly(tokenMint, false),
                AccountMeta.Writable(destination, false),
            ];
            keys = AddSigners(keys, authority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeTransferCheckedData(amount, decimals)
            };
        }

        /// <summary>
        /// Initializes a new token account with the given mint and authority.
        /// </summary>
        /// <param name="account">The token account to initialize.</param>
        /// <param name="mint">The mint associated with the token account.</param>
        /// <param name="authority">The authority for the token account.</param>
        /// <returns>The transaction instruction to initialize the token account.</returns>
        public static TransactionInstruction InitializeAccount(PublicKey account, PublicKey mint, PublicKey authority)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(authority, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeAccountData()
            };
        }


        /// <summary>
        /// Initializes a new token account with the given mint and owner.
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mint"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static TransactionInstruction InitializeAccount2(PublicKey account, PublicKey mint, PublicKey owner)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeAccount2Data(owner)
            };
        }

        /// <summary>
        /// Initializes a new token account with the given mint and owner.
        /// </summary>
        /// <param name="account">The token account to initialize.</param>
        /// <param name="mint">The mint associated with the token account.</param>
        /// <param name="owner">The owner of the token account.</param>
        /// <returns>The transaction instruction to initialize the token account.</returns>
        public static TransactionInstruction InitializeAccount3(PublicKey account, PublicKey mint, PublicKey owner)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
                AccountMeta.ReadOnly(mint, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeAccount3Data(owner)
            };
        }

        /// <summary>
        /// Initializes a new multisignature account with the given signers and threshold.
        /// </summary>
        /// <param name="multiSignature">The multisignature account to initialize.</param>
        /// <param name="signers">The signers for the multisignature account.</param>
        /// <param name="m">The threshold for the multisignature account.</param>
        /// <returns>The transaction instruction to initialize the multisignature account.</returns>
        public static TransactionInstruction InitializeMultiSignature(PublicKey multiSignature,
            IEnumerable<PublicKey> signers, int m)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(multiSignature, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                .. signers.Select(signer => AccountMeta.ReadOnly(signer, false)),
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMultiSignatureData(m)
            };
        }

        /// <summary>
        /// Initializes a new multisignature account with the given signers and threshold.
        /// </summary>
        /// <param name="multiSignature">The multisignature account to initialize.</param>
        /// <param name="signers">The signers for the multisignature account.</param>
        /// <param name="m">The threshold for the multisignature account.</param>
        /// <returns>The transaction instruction to initialize the multisignature account.</returns>
        public static TransactionInstruction InitializeMultiSignature2(PublicKey multiSignature,
            IEnumerable<PublicKey> signers, int m)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(multiSignature, false), .. signers.Select(signer => AccountMeta.ReadOnly(signer, false))
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMultiSignature2Data(m)
            };
        }

        /// <summary>
        /// Initializes a new mint with the given parameters.
        /// </summary>
        /// <param name="mint">The mint account to initialize.</param>
        /// <param name="decimals">The number of decimals for the mint.</param>
        /// <param name="mintAuthority">The authority allowed to mint new tokens.</param>
        /// <param name="freezeAuthority">The authority allowed to freeze token accounts.</param>
        /// <returns>The transaction instruction to initialize the mint.</returns>
        public static TransactionInstruction InitializeMint(PublicKey mint, int decimals, PublicKey mintAuthority,
            PublicKey freezeAuthority = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(mint, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false)
            ];

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
        /// Initializes a new mint with the given parameters.
        /// </summary>
        /// <param name="mint">The mint account to initialize.</param>
        /// <param name="decimals">The number of decimals for the mint.</param>
        /// <param name="mintAuthority">The authority allowed to mint new tokens.</param>
        /// <param name="freezeAuthority">The authority allowed to freeze token accounts.</param>
        /// <returns>The transaction instruction to initialize the mint.</returns>
        public static TransactionInstruction InitializeMint2(PublicKey mint, int decimals, PublicKey mintAuthority,
            PublicKey freezeAuthority = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(mint, false)
            ];

            int freezeAuthorityOpt = freezeAuthority != null ? 1 : 0;
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMint2Data(
                    mintAuthority,
                    freezeAuthority ?? new Account().PublicKey,
                    decimals,
                    freezeAuthorityOpt)
            };
        }

        /// <summary>
        /// Mints new tokens to a specified destination account.
        /// </summary>
        /// <param name="mint">The mint account from which tokens will be minted.</param>
        /// <param name="destination">The destination account to receive the minted tokens.</param>
        /// <param name="amount">The amount of tokens to mint.</param>
        /// <param name="mintAuthority">The authority allowed to mint new tokens.</param>
        /// <param name="signers">The signers for the minting transaction.</param>
        /// <returns></returns>
        public static TransactionInstruction MintTo(PublicKey mint, PublicKey destination, ulong amount,
            PublicKey mintAuthority, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(mint, false),
                AccountMeta.Writable(destination, false),
            ];
            keys = AddSigners(keys, mintAuthority, signers);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeMintToData(amount)
            };
        }
        /// <summary>
        /// Approves a delegate to transfer tokens from a source account.
        /// </summary>
        /// <param name="source">The source account from which tokens will be transferred.</param>
        /// <param name="delegatePublicKey">The delegate account authorized to transfer tokens.</param>
        /// <param name="authority">The authority account approving the delegate.</param>
        /// <param name="amount">The amount of tokens the delegate is allowed to transfer.</param>
        /// <param name="signers">The signers for the approval transaction.</param>
        /// <returns>The transaction instruction to approve the delegate.</returns>
        public static TransactionInstruction Approve(
            PublicKey source, PublicKey delegatePublicKey, PublicKey authority, ulong amount,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
                AccountMeta.ReadOnly(delegatePublicKey, false)
            ];

            keys = AddSigners(keys, authority, signers);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeApproveData(amount)
            };
        }

        /// <summary>
        /// Revokes the delegate's authority to transfer tokens from a source account.
        /// </summary>
        /// <param name="source">The source account from which tokens will be transferred.</param>
        /// <param name="authority">The authority account revoking the delegate's permission.</param>
        /// <param name="signers">The signers for the revoke transaction.</param>
        /// <returns>The transaction instruction to revoke the delegate's authority.</returns>
        public static TransactionInstruction Revoke(PublicKey source, PublicKey authority,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
            ];
            keys = AddSigners(keys, authority, signers);

            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeRevokeData()
            };
        }

        /// <summary>
        /// Sets a new authority for a given account, allowing for changes in control over the account's operations.
        /// </summary>
        /// <param name="account">The account for which the authority is being set.</param>
        /// <param name="authority">The type of authority being set.</param>
        /// <param name="currentAuthority">The current authority of the account.</param>
        /// <param name="newAuthority">The new authority to be set for the account.</param>
        /// <param name="signers">The signers for the set authority transaction.</param>
        /// <returns>The transaction instruction to set the new authority.</returns>
        public static TransactionInstruction SetAuthority(
            PublicKey account, AuthorityType authority, PublicKey currentAuthority, PublicKey newAuthority = null,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
            ];
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
        /// Burns a specified amount of tokens from a source account, reducing the total supply of tokens.
        /// </summary>
        /// <param name="source">The source account from which tokens will be burned.</param>
        /// <param name="mint">The mint account associated with the tokens.</param>
        /// <param name="amount">The amount of tokens to burn.</param>
        /// <param name="authority">The authority account approving the burn.</param>
        /// <param name="signers">The signers for the burn transaction.</param>
        /// <returns>The transaction instruction to burn the specified amount of tokens.</returns>
        public static TransactionInstruction Burn(PublicKey source, PublicKey mint, ulong amount, PublicKey authority,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
                AccountMeta.Writable(mint, false),
            ];
            keys = AddSigners(keys, authority, signers);
            return new TransactionInstruction()
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeBurnData(amount)
            };
        }

        /// <summary>
        /// Closes a token account, transferring any remaining balance to a destination account and deallocating the account's data.
        /// </summary>
        /// <param name="account">The token account to be closed.</param>
        /// <param name="destination">The account to receive any remaining balance.</param>
        /// <param name="authority">The authority account approving the closure.</param>
        /// <param name="signers">The signers for the close account transaction.</param>
        /// <returns>The transaction instruction to close the token account.</returns>
        public static TransactionInstruction CloseAccount(PublicKey account, PublicKey destination, PublicKey authority,
            IEnumerable<PublicKey> signers = null)
            => TokenProgram.CloseAccount(account, destination, authority, ProgramIdKey, signers);


        /// <summary>
        /// Freezes a token account, preventing any further transfers or operations on the account until it is thawed.
        /// </summary>
        /// <param name="account">The token account to be frozen.</param>
        /// <param name="mint">The mint account associated with the token.</param>
        /// <param name="freezeAuthority">The authority account approving the freeze.</param>
        /// <param name="signers">The signers for the freeze account transaction.</param>
        /// <returns>The transaction instruction to freeze the token account.</returns>
        public static TransactionInstruction FreezeAccount(PublicKey account, PublicKey mint, PublicKey freezeAuthority,
            IEnumerable<PublicKey> signers = null)
            => TokenProgram.FreezeAccount(account, mint, freezeAuthority, ProgramIdKey, signers);

        /// <summary>
        /// Thaws a previously frozen token account, allowing transfers and operations to resume.
        /// </summary>
        /// <param name="account">The token account to be thawed.</param>
        /// <param name="mint">The mint account associated with the token.</param>
        /// <param name="freezeAuthority">The authority account approving the thaw.</param>
        /// <param name="signers">The signers for the thaw account transaction.</param>
        /// <returns>The transaction instruction to thaw the token account.</returns>
        public static TransactionInstruction ThawAccount(PublicKey account, PublicKey mint, PublicKey freezeAuthority,
            IEnumerable<PublicKey> signers = null)
            => TokenProgram.ThawAccount(account, mint, freezeAuthority, ProgramIdKey, signers);

        /// <summary>
        /// Approves a delegate to transfer tokens from a source account, checking the number of decimals.
        /// </summary>
        /// <param name="source">The source account from which tokens will be transferred.</param>
        /// <param name="delegatePublicKey">The delegate account authorized to transfer tokens.</param>
        /// <param name="amount">The amount of tokens the delegate is allowed to transfer.</param>
        /// <param name="decimals">The number of decimals to consider for the token amount.</param>
        /// <param name="authority">The authority account approving the delegate.</param>
        /// <param name="mint">The mint account associated with the token.</param>
        /// <param name="signers">The signers for the approve checked transaction.</param>
        /// <returns>The transaction instruction to approve the delegate.</returns>
        public static TransactionInstruction ApproveChecked(
            PublicKey source, PublicKey delegatePublicKey, ulong amount, byte decimals, PublicKey authority, PublicKey mint,
            IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(source, false),
                AccountMeta.ReadOnly(mint, false),
                AccountMeta.ReadOnly(delegatePublicKey, false),
            ];
            keys = AddSigners(keys, authority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeApproveCheckedData(amount, decimals)
            };
        }
        /// <summary>
        /// Mints new tokens to a specified destination account, checking the number of decimals.
        /// </summary>
        /// <param name="mint">The mint account from which tokens will be minted.</param>
        /// <param name="destination">The destination account to receive the minted tokens.</param>
        /// <param name="mintAuthority">The authority account approving the minting.</param>
        /// <param name="amount">The amount of tokens to be minted.</param>
        /// <param name="decimals">The number of decimals to consider for the token amount.</param>
        /// <param name="signers">The signers for the mint to checked transaction.</param>
        /// <returns>The transaction instruction to mint the tokens.</returns>
        public static TransactionInstruction MintToChecked(PublicKey mint, PublicKey destination,
            PublicKey mintAuthority, ulong amount, int decimals, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(mint, false),
                AccountMeta.Writable(destination, false),
            ];
            keys = AddSigners(keys, mintAuthority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeMintToCheckedData(amount, decimals)
            };
        }
        /// <summary>
        /// Burns a specified amount of tokens from a source account, checking the number of decimals.
        /// </summary>
        /// <param name="mint">The mint account associated with the token.</param>
        /// <param name="account">The source account from which tokens will be burned.</param>
        /// <param name="authority">The authority account approving the burn.</param>
        /// <param name="amount">The amount of tokens to be burned.</param>
        /// <param name="decimals">The number of decimals to consider for the token amount.</param>
        /// <param name="signers">The signers for the burn checked transaction.</param>
        /// <returns>The transaction instruction to burn the tokens.</returns>
        public static TransactionInstruction BurnChecked(PublicKey mint, PublicKey account, PublicKey authority,
            ulong amount, int decimals, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
                AccountMeta.Writable(mint, false),
            ];
            keys = AddSigners(keys, authority, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeBurnCheckedData(amount, decimals)
            };
        }
        /// <summary>
        /// Synchronizes the native token account with the underlying native token balance, ensuring that the account's state reflects the actual balance of native tokens.
        /// </summary>
        /// <param name="account">The native token account to be synchronized.</param>
        /// <returns>The transaction instruction to synchronize the native token account.</returns>
        public static TransactionInstruction SyncNative(PublicKey account)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeSyncNativeData()
            };
        }
        /// <summary>
        /// Initializes a token account with an immutable owner, preventing any future changes to the account's ownership.
        /// </summary>
        /// <param name="account">The token account to be initialized with an immutable owner.</param>
        /// <returns>The transaction instruction to initialize the token account with an immutable owner.</returns>
        public static TransactionInstruction InitializeImmutableOwner(PublicKey account)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeImmutableOwnerData()
            };
        }
        /// <summary>
        /// Retrieves the size of the account data for a given mint and its associated extension types, allowing for dynamic allocation of account space based on the extensions used.
        /// </summary>
        /// <param name="mint">The mint for which to retrieve the account data size.</param>
        /// <param name="extensionTypes">The extension types associated with the mint.</param>
        /// <returns>The transaction instruction to retrieve the account data size.</returns>
        public static TransactionInstruction GetAccountDataSize(PublicKey mint,
            IEnumerable<Token2022ExtensionType> extensionTypes)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.ReadOnly(mint, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeGetAccountDataSizeData(extensionTypes)
            };
        }
        /// <summary>
        /// Converts a specified amount of tokens to its corresponding UI amount based on the mint's decimal configuration, facilitating user-friendly display of token balances.
        /// </summary>
        /// <param name="mint">The mint for which to convert the amount.</param>
        /// <param name="amount">The amount of tokens to be converted.</param>
        /// <returns>The transaction instruction to convert the amount to a UI amount.</returns>
        public static TransactionInstruction AmountToUiAmount(PublicKey mint, ulong amount)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.ReadOnly(mint, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeAmountToUiAmountData(amount)
            };
        }
        /// <summary>
        /// Converts a specified UI amount to its corresponding token amount based on the mint's decimal configuration, allowing for accurate representation of user inputs in token transactions.
        /// </summary>
        /// <param name="mint">The mint for which to convert the UI amount.</param>
        /// <param name="uiAmount">The UI amount to be converted.</param>
        /// <returns>The transaction instruction to convert the UI amount to a token amount.</returns>
        public static TransactionInstruction UiAmountToAmount(PublicKey mint, string uiAmount)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.ReadOnly(mint, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeUiAmountToAmountData(uiAmount)
            };
        }
        /// <summary>
        /// Initializes the close authority for a mint, allowing for the specification of an authority that can close the mint account and reclaim any remaining lamports.
        /// </summary>
        /// <param name="mint">The mint for which to initialize the close authority.</param>
        /// <param name="closeAuthority">The close authority to be set for the mint.</param>
        /// <returns>The transaction instruction to initialize the mint close authority.</returns>
        public static TransactionInstruction InitializeMintCloseAuthority(PublicKey mint, PublicKey closeAuthority = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(mint, false)
            ];
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeInitializeMintCloseAuthorityData(closeAuthority)
            };
        }
        /// <summary>
        /// Reallocates the account data for a given token account, allowing for the addition or removal of extension types and adjusting the account's size accordingly.
        /// </summary>
        /// <param name="account">The token account to be reallocated.</param>
        /// <param name="payer">The account responsible for paying the reallocation fees.</param>
        /// <param name="owner">The owner of the token account.</param>
        /// <param name="extensionTypes">The extension types to be added or removed.</param>
        /// <param name="signers">Optional signers for the transaction.</param>
        /// <returns>The transaction instruction to reallocate the token account.</returns>
        public static TransactionInstruction Reallocate(PublicKey account, PublicKey payer, PublicKey owner,
            IEnumerable<Token2022ExtensionType> extensionTypes, IEnumerable<PublicKey> signers = null)
        {
            List<AccountMeta> keys =
            [
                AccountMeta.Writable(account, false),
                AccountMeta.Writable(payer, true),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
            ];
            keys = AddSigners(keys, owner, signers);
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = TokenProgramData.EncodeReallocateData(extensionTypes)
            };
        }
        /// <summary>
        /// Adds the authority and signers to the list of account metadata for a transaction instruction, ensuring that the appropriate accounts are included for signing and authorization.
        /// </summary>
        /// <param name="keys">The list of account metadata to which the authority and signers will be added.</param>
        /// <param name="authority">The authority account for the transaction.</param>
        /// <param name="signers">Optional signers for the transaction.</param>
        /// <returns>The updated list of account metadata including the authority and signers.</returns>
        private static List<AccountMeta> AddSigners(List<AccountMeta> keys, PublicKey authority = null,
            IEnumerable<PublicKey> signers = null)
        {
            if (signers != null)
            {
                keys.Add(AccountMeta.ReadOnly(authority, false));
                keys.AddRange(signers.Select(signer => AccountMeta.ReadOnly(signer, true)));
            }
            else
            {
                keys.Add(AccountMeta.ReadOnly(authority, true));
            }

            return keys;
        }
        /// <summary>
        /// Decodes a transaction instruction for the Token Program, extracting the instruction name, program name, and associated values based on the provided data and keys.
        /// </summary>
        /// <param name="data">The raw instruction data.</param>
        /// <param name="keys">The list of public keys involved in the instruction.</param>
        /// <param name="keyIndices">The indices of the keys in the instruction data.</param>
        /// <returns>The decoded instruction.</returns>
        public static DecodedInstruction Decode(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            byte instruction = data.GetU8(TokenProgramData.MethodOffset);

            if (!Enum.IsDefined(typeof(TokenProgramInstructions.Values), instruction))
            {
                return new()
                {
                    PublicKey = ProgramIdKey,
                    InstructionName = "Unknown Instruction",
                    ProgramName = ProgramName,
                    Values = [],
                    InnerInstructions = []
                };
            }

            TokenProgramInstructions.Values instructionValue = (TokenProgramInstructions.Values)instruction;

            DecodedInstruction decodedInstruction = new()
            {
                PublicKey = ProgramIdKey,
                InstructionName = TokenProgramInstructions.Names[instructionValue],
                ProgramName = ProgramName,
                Values = [],
                InnerInstructions = []
            };

            switch (instructionValue)
            {
                case TokenProgramInstructions.Values.InitializeMint:
                    TokenProgramData.DecodeInitializeMintData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeAccount:
                    TokenProgramData.DecodeInitializeAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeMultiSignature:
                    TokenProgramData.DecodeInitializeMultiSignatureData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.Transfer:
                    TokenProgramData.DecodeTransferData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.Approve:
                    TokenProgramData.DecodeApproveData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.Revoke:
                    TokenProgramData.DecodeRevokeData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.SetAuthority:
                    TokenProgramData.DecodeSetAuthorityData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.MintTo:
                    TokenProgramData.DecodeMintToData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.Burn:
                    TokenProgramData.DecodeBurnData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.CloseAccount:
                    TokenProgramData.DecodeCloseAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.FreezeAccount:
                    TokenProgramData.DecodeFreezeAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.ThawAccount:
                    TokenProgramData.DecodeThawAccountData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.TransferChecked:
                    TokenProgramData.DecodeTransferCheckedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.ApproveChecked:
                    TokenProgramData.DecodeApproveCheckedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.MintToChecked:
                    TokenProgramData.DecodeMintToCheckedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.BurnChecked:
                    TokenProgramData.DecodeBurnCheckedData(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.SyncNative:
                    TokenProgramData.DecodeSyncNativeData(decodedInstruction, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeAccount2:
                    TokenProgramData.DecodeInitializeAccount2(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeAccount3:
                    TokenProgramData.DecodeInitializeAccount3(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeMint2:
                    TokenProgramData.DecodeInitializeMint2(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeMultiSignature2:
                    TokenProgramData.DecodeInitializeMultiSignature2(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.GetAccountDataSize:
                    TokenProgramData.DecodeGetAccountDataSize(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeImmutableOwner:
                    TokenProgramData.DecodeInitializeImmutableOwner(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.AmountToUiAmount:
                    TokenProgramData.DecodeAmountToUiAmount(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.UiAmountToAmount:
                    TokenProgramData.DecodeUiAmountToAmount(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.InitializeMintCloseAuthority:
                    TokenProgramData.DecodeInitializeMintCloseAuthority(decodedInstruction, data, keys, keyIndices);
                    break;
                case TokenProgramInstructions.Values.Reallocate:
                    TokenProgramData.DecodeReallocate(decodedInstruction, data, keys, keyIndices);
                    break;
            }
            return decodedInstruction;
        }
    }
}