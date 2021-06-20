// unset

using System;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the token program data encodings.
    /// </summary>
    internal static class TokenProgramData
    {
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Revoke"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRevokeData() => new[] { (byte)TokenProgramInstructions.Revoke };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Approve"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens to approve the transfer of.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeApproveData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Approve, amount);
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.InitializeAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeAccountData() => new[] { (byte)TokenProgramInstructions.InitializeAccount };

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.InitializeMint"/> method.
        /// </summary>
        /// <param name="mintAuthority">The mint authority for the token.</param>
        /// <param name="freezeAuthority">The freeze authority for the token.</param>
        /// <param name="decimals">The amount of decimals.</param>
        /// <param name="freezeAuthorityOption">The freeze authority option for the token.</param>
        /// <remarks>The <c>freezeAuthorityOption</c> parameter is related to the existence or not of a freeze authority.</remarks>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMintData(
            byte[] mintAuthority, byte[] freezeAuthority, int decimals, int freezeAuthorityOption)
        {
            byte[] methodBuffer = new byte[67];

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
        internal static byte[] EncodeTransferData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Transfer, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.TransferChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The number of decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeTransferCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.TransferChecked, amount, (byte)decimals);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.MintTo"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMintToData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.MintTo, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.InitializeMultiSignature"/> method.
        /// </summary>
        /// <param name="m">The number of signers necessary to validate the account.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMultiSignatureData(int m)
        {
            byte[] methodBuffer = new byte[2];
            
            methodBuffer[0] = (byte)TokenProgramInstructions.InitializeMultiSignature;
            methodBuffer[1] = (byte)m;

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.SetAuthority"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSetAuthorityData(AuthorityType authorityType, int newAuthorityOption, byte[] newAuthority)
        {
            byte[] methodBuffer = new byte[34];

            methodBuffer[0] = (byte)TokenProgramInstructions.SetAuthority;
            methodBuffer[1] = (byte)authorityType;
            methodBuffer[2] = (byte)newAuthorityOption;
            Array.Copy(newAuthority, 0, methodBuffer, 3, 32);

            return methodBuffer;
        }
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Burn"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeBurnData(ulong amount) 
            => EncodeAmountLayout((byte)TokenProgramInstructions.Burn, amount);
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.CloseAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCloseAccountData() => new[] { (byte)TokenProgramInstructions.CloseAccount };
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.FreezeAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeFreezeAccountData() => new[] { (byte)TokenProgramInstructions.FreezeAccount };
        
        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.ThawAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeThawAccountData() => new[] { (byte)TokenProgramInstructions.ThawAccount };
        
        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.ApproveChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeApproveCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.ApproveChecked, amount, (byte)decimals);
        
        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.MintToChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMintToCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.MintToChecked, amount, (byte)decimals);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.BurnChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeBurnCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.BurnChecked, amount, (byte)decimals);
        
        /// <summary>
        /// Encodes the transaction data for the methods which only require the amount.
        /// </summary>
        /// <param name="method">The method identifier.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeAmountLayout(byte method, ulong amount)
        {
            byte[] methodBuffer = new byte[9];
            methodBuffer[0] = method;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);
            return methodBuffer;
        }
        
        /// <summary>
        /// Encodes the transaction data for the methods which only require the amount and the number of decimals.
        /// </summary>
        /// <param name="method">The method identifier.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeAmountCheckedLayout(byte method, ulong amount, byte decimals)
        {
            byte[] methodBuffer = new byte[10];
            methodBuffer[0] = method;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);
            methodBuffer[9] = decimals;
            return methodBuffer;
        }
    }
}