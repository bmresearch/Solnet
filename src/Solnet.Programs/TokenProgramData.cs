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
        internal static byte[] EncodeRevokeData()
        {
            byte[] methodBuffer = new byte[1];
            methodBuffer[0] = (byte)TokenProgramInstructions.Revoke;
            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Approve"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens to approve the transfer of.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeApproveData(long amount)
        {
            byte[] methodBuffer = new byte[9];

            methodBuffer[0] = (byte)TokenProgramInstructions.Approve;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);
            return methodBuffer;
        }

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
        internal static byte[] EncodeTransferData(long amount)
        {
            byte[] methodBuffer = new byte[9];

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
        internal static byte[] EncodeTransferCheckedData(long amount, byte decimals)
        {
            byte[] methodBuffer = new byte[10];

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
        internal static byte[] EncodeMintToData(long amount)
        {
            byte[] methodBuffer = new byte[9];

            methodBuffer[0] = (byte)TokenProgramInstructions.MintTo;
            Utils.Int64ToByteArrayLe(amount, methodBuffer, 1);

            return methodBuffer;
        }
        
    }
}