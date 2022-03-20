using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the BPF Loader Program methods.
    /// <remarks>
    /// For more information see:
    /// https://docs.rs/solana-sdk/1.9.13/solana_sdk/loader_upgradeable_instruction/enum.UpgradeableLoaderInstruction.html
    /// </remarks>
    /// </summary>
    public static class BPFLoaderProgram 
    {
        /// <summary>
        /// The public key of the BPF Loader Program.
        /// </summary>
        public static readonly PublicKey ProgramIdKey = new("BPFLoaderUpgradeab1e11111111111111111111111");
        /// <summary>
        /// The program's name.
        /// </summary>
        private const string ProgramName = "BPF Loader Program";
        /// <summary>
        /// Initialize a Buffer account.
        /// A Buffer account is an intermediary that once fully populated is used with the DeployWithMaxDataLen instruction to populate the program’s ProgramData account.
        /// The InitializeBuffer instruction requires no signers and MUST be included within the same Transaction as the system program’s CreateAccount instruction that creates the account being initialized. Otherwise another party may initialize the account.
        /// </summary>
        /// <param name="sourceAccount">public key of the account to init</param>
        /// <param name="authority">public key of the authority over the account</param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction InitializeBuffer(PublicKey  sourceAccount  , PublicKey authority = null)
        {
          var keys = new  List<AccountMeta> ()
            {
                AccountMeta.Writable(sourceAccount, false),
            };
          if (authority !=null) keys.Add( AccountMeta.ReadOnly(authority,false));
          return new TransactionInstruction
          {
              ProgramId = ProgramIdKey.KeyBytes,
              Keys = keys,
              Data = BPFLoaderProgramData.EncodeInitializeBuffer().ToArray()
          };
        }

        /// <summary>
        /// Write program data into a Buffer account.
        /// </summary>
        /// <param name="bufferAccount">the public key of the buffer account</param>
        /// <param name="bufferAuthority">the public key of the authority over the buffer account</param>
        /// <param name="data">data to write to the buffer account (Serialized program data)</param>
        /// <param name="offset">offset at which to write the given data.</param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction Write(PublicKey bufferAccount, PublicKey bufferAuthority,   Span<byte> data, uint offset)
        {
            var keys = new  List<AccountMeta> ()
            {
                AccountMeta.Writable(bufferAccount, false),
                AccountMeta.ReadOnly(bufferAuthority, true),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = BPFLoaderProgramData.EncodeWrite(offset, data).ToArray()
            };
        }
        /// <summary>
        ///A program consists of a Program and ProgramData account pair
        /// The Program account’s address will serve as the program id for any instructions that execute this program.
        /// The ProgramData account will remain mutable by the loader only and holds the program data and authority information. The ProgramData account’s address is derived from the Program account’s address and created by the DeployWithMaxDataLen instruction
        /// </summary>
        /// <param name="payer">The payer account that will pay to create the ProgramData account</param>
        /// <param name="programDataAccount">The uninitialized ProgramData account</param>
        /// <param name="programAccount">The uninitialized Program account</param>
        /// <param name="bufferAccount"> The Buffer account where the program data has been written. The buffer account’s authority must match the program’s authority</param>
        /// <param name="authority">the public key of the authority</param>
        /// <param name="maxDataLenght">Maximum length that the program can be upgraded to</param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction DeployWithMaxDataLen(PublicKey payer, PublicKey programDataAccount,
            PublicKey programAccount, PublicKey bufferAccount, PublicKey authority, ulong maxDataLenght)
        {
            var keys = new  List<AccountMeta> ()
            {
                AccountMeta.ReadOnly(payer, true),
                AccountMeta.Writable(programDataAccount, false),
                AccountMeta.Writable(programAccount, false),
                AccountMeta.Writable(bufferAccount, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(SystemProgram.ProgramIdKey, false),
                AccountMeta.ReadOnly(authority, true),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = BPFLoaderProgramData.EncodeDeployWithMaxDataLen(maxDataLenght).ToArray()
            };
        }
        /// <summary>
        /// Upgrade a program
        ///A program can be updated as long as the program’s authority has not been set to None.
        /// The Buffer account must contain sufficient lamports to fund the ProgramData account to be rent-exempt, any additional lamports left over will be transferred to the spill account, leaving the Buffer account balance at zero.
        /// </summary>
        /// <param name="programDataAccount">The ProgramData account.</param>
        /// <param name="programAccount"></param>
        /// <param name="bufferAccount"></param>
        /// <param name="spillAccount"></param>
        /// <param name="authority"></param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction Upgrade(PublicKey programDataAccount, PublicKey programAccount,
            PublicKey bufferAccount, PublicKey spillAccount, PublicKey authority)
        {
            var keys = new  List<AccountMeta> ()
            {
                AccountMeta.Writable(programDataAccount, false),
                AccountMeta.Writable(programAccount, false),
                AccountMeta.Writable(bufferAccount, false),
                AccountMeta.Writable(spillAccount, false),
                AccountMeta.ReadOnly(SysVars.RentKey, false),
                AccountMeta.ReadOnly(SysVars.ClockKey, false),
                AccountMeta.ReadOnly(authority, true),
            };
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = BPFLoaderProgramData.EncodeUpgrade().ToArray()
            };
        }

        /// <summary>
        ///Set a new authority that is allowed to write the buffer or upgrade the program. To permanently make the buffer immutable or disable program updates omit the new authority.
        /// </summary>
        /// <param name="bufferOrProgramDataaccount"></param>
        /// <param name="authority"></param>
        /// <param name="newAuthority"></param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction SetAuthority(PublicKey bufferOrProgramDataaccount, PublicKey authority,
            PublicKey newAuthority = null)
        {
            var keys = new  List<AccountMeta> ()
            {
                AccountMeta.Writable(bufferOrProgramDataaccount, false),
                AccountMeta.ReadOnly(authority, true),
            };
            if (newAuthority != null) keys.Add(AccountMeta.ReadOnly(newAuthority,false));
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = BPFLoaderProgramData.EncodeSetAuthority().ToArray()
            };
        }
        /// <summary>
        /// Closes an account owned by the upgradeable loader of all lamports and withdraws all the lamports
        /// </summary>
        /// <param name="accountToClose">public key of the account to close</param>
        /// <param name="depositAccount">public key of the account to transfer remaining lamports to </param>
        /// <param name="associatedProgramAccount">public key of the associated program account</param>
        /// <param name="authority">public key of the authority for the account to close</param>
        /// <returns>The transaction instruction</returns>
        public static TransactionInstruction Close(PublicKey accountToClose, PublicKey depositAccount,
            PublicKey associatedProgramAccount = null, PublicKey authority = null)
        {
            var keys = new  List<AccountMeta> ()
            {
                AccountMeta.Writable(accountToClose, false),
                AccountMeta.Writable(depositAccount, false),
            };
            if (authority != null) keys.Add(AccountMeta.ReadOnly(authority, true));
            if (associatedProgramAccount != null) keys.Add(AccountMeta.Writable(associatedProgramAccount,false));
            return new TransactionInstruction
            {
                ProgramId = ProgramIdKey.KeyBytes,
                Keys = keys,
                Data = BPFLoaderProgramData.EncodeClose().ToArray()
            };
        }
    }
}

