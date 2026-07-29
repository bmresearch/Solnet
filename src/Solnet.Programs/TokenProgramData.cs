using Solnet.Programs.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solnet.Programs
{
    /// <summary>
    /// Implements the token program data encodings.
    /// </summary>
    internal static class TokenProgramData
    {
        /// <summary>
        /// The offset at which the value which defines the method begins.
        /// </summary>
        internal const int MethodOffset = 0;

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.Revoke"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeRevokeData() => [(byte)TokenProgramInstructions.Values.Revoke];

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.Approve"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens to approve the transfer of.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeApproveData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Values.Approve, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeAccountData() =>
            [(byte)TokenProgramInstructions.Values.InitializeAccount];

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMint"/> method.
        /// </summary>
        /// <param name="mintAuthority">The mint authority for the token.</param>
        /// <param name="freezeAuthority">The freeze authority for the token.</param>
        /// <param name="decimals">The amount of decimals.</param>
        /// <param name="freezeAuthorityOption">The freeze authority option for the token.</param>
        /// <remarks>The <c>freezeAuthorityOption</c> parameter is related to the existence or not of a freeze authority.</remarks>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMintData(
            PublicKey mintAuthority, PublicKey freezeAuthority, int decimals, int freezeAuthorityOption)
        {
            byte[] methodBuffer = new byte[67];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.InitializeMint, MethodOffset);
            methodBuffer.WriteU8((byte)decimals, 1);
            methodBuffer.WritePubKey(mintAuthority, 2);
            methodBuffer.WriteU8((byte)freezeAuthorityOption, 34);
            methodBuffer.WritePubKey(freezeAuthority, 35);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.Transfer"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeTransferData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Values.Transfer, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.TransferChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The number of decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeTransferCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.Values.TransferChecked, amount, (byte)decimals);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.MintTo"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMintToData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Values.MintTo, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMultiSignature"/> method.
        /// </summary>
        /// <param name="m">The number of signers necessary to validate the account.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMultiSignatureData(int m)
        {
            byte[] methodBuffer = new byte[2];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.InitializeMultiSignature, MethodOffset);
            methodBuffer.WriteU8((byte)m, 1);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.SetAuthority"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSetAuthorityData(AuthorityType authorityType, int newAuthorityOption,
            PublicKey newAuthority)
        {
            byte[] methodBuffer = new byte[35];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.SetAuthority, MethodOffset);
            methodBuffer.WriteU8((byte)authorityType, 1);
            methodBuffer.WriteU8((byte)newAuthorityOption, 2);
            methodBuffer.WritePubKey(newAuthority, 3);

            return methodBuffer;
        }

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.Burn"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeBurnData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Values.Burn, amount);

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.CloseAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeCloseAccountData() => [(byte)TokenProgramInstructions.Values.CloseAccount];

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.FreezeAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeFreezeAccountData() =>
            [(byte)TokenProgramInstructions.Values.FreezeAccount];

        /// <summary>
        /// Encode the transaction instruction data for the <see cref="TokenProgramInstructions.Values.ThawAccount"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeThawAccountData() => [(byte)TokenProgramInstructions.Values.ThawAccount];

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.ApproveChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeApproveCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.Values.ApproveChecked, amount, (byte)decimals);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.MintToChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeMintToCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.Values.MintToChecked, amount, (byte)decimals);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.BurnChecked"/> method.
        /// </summary>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeBurnCheckedData(ulong amount, int decimals)
            => EncodeAmountCheckedLayout((byte)TokenProgramInstructions.Values.BurnChecked, amount, (byte)decimals);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.SyncNative"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeSyncNativeData() =>
            [(byte) TokenProgramInstructions.Values.SyncNative];

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeAccount2"/> method.
        /// </summary>
        /// <param name="owner">The owner public key.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeAccount2Data(PublicKey owner)
            => EncodeInitializeAccountOwnerData((byte)TokenProgramInstructions.Values.InitializeAccount2, owner);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeAccount3"/> method.
        /// </summary>
        /// <param name="owner">The owner public key.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeAccount3Data(PublicKey owner)
            => EncodeInitializeAccountOwnerData((byte)TokenProgramInstructions.Values.InitializeAccount3, owner);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMultiSignature2"/> method.
        /// </summary>
        /// <param name="m">The number of required signers.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMultiSignature2Data(int m)
        {
            byte[] methodBuffer = new byte[2];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.InitializeMultiSignature2, MethodOffset);
            methodBuffer.WriteU8((byte)m, 1);

            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMint2"/> method.
        /// </summary>
        /// <param name="mintAuthority">The mint authority.</param>
        /// <param name="freezeAuthority">The freeze authority.</param>
        /// <param name="decimals">The number of decimals.</param>
        /// <param name="freezeAuthorityOption">The freeze authority option.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMint2Data(
            PublicKey mintAuthority, PublicKey freezeAuthority, int decimals, int freezeAuthorityOption)
        {
            byte[] methodBuffer = new byte[67];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.InitializeMint2, MethodOffset);
            methodBuffer.WriteU8((byte)decimals, 1);
            methodBuffer.WritePubKey(mintAuthority, 2);
            methodBuffer.WriteU8((byte)freezeAuthorityOption, 34);
            methodBuffer.WritePubKey(freezeAuthority, 35);

            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.GetAccountDataSize"/> method.
        /// </summary>
        /// <param name="extensionTypes">The extension types to include.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeGetAccountDataSizeData(IEnumerable<Token2022ExtensionType> extensionTypes)
        {
            Token2022ExtensionType[] extensions = extensionTypes?.ToArray() ?? [];
            byte[] methodBuffer = new byte[1 + extensions.Length * 2];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.GetAccountDataSize, MethodOffset);

            for (int i = 0; i < extensions.Length; i++)
            {
                methodBuffer.WriteU16((ushort)extensions[i], 1 + i * 2);
            }

            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeImmutableOwner"/> method.
        /// </summary>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeImmutableOwnerData()
            => [(byte)TokenProgramInstructions.Values.InitializeImmutableOwner];

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.AmountToUiAmount"/> method.
        /// </summary>
        /// <param name="amount">The amount.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeAmountToUiAmountData(ulong amount)
            => EncodeAmountLayout((byte)TokenProgramInstructions.Values.AmountToUiAmount, amount);

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.UiAmountToAmount"/> method.
        /// </summary>
        /// <param name="uiAmount">The ui amount string.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeUiAmountToAmountData(string uiAmount)
        {
            byte[] uiAmountBytes = Encoding.UTF8.GetBytes(uiAmount);
            byte[] methodBuffer = new byte[1 + uiAmountBytes.Length];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.UiAmountToAmount, MethodOffset);
            Array.Copy(uiAmountBytes, 0, methodBuffer, 1, uiAmountBytes.Length);

            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMintCloseAuthority"/> method.
        /// </summary>
        /// <param name="closeAuthority">The close authority, if present.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeInitializeMintCloseAuthorityData(PublicKey closeAuthority)
        {
            if (closeAuthority == null)
            {
                return [(byte)TokenProgramInstructions.Values.InitializeMintCloseAuthority, (byte)0];
            }

            byte[] methodBuffer = new byte[34];
            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.InitializeMintCloseAuthority, MethodOffset);
            methodBuffer.WriteU8(1, 1);
            methodBuffer.WritePubKey(closeAuthority, 2);
            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the <see cref="TokenProgramInstructions.Values.Reallocate"/> method.
        /// </summary>
        /// <param name="extensionTypes">The extension types to include.</param>
        /// <returns>The byte array with the encoded data.</returns>
        internal static byte[] EncodeReallocateData(IEnumerable<Token2022ExtensionType> extensionTypes)
        {
            Token2022ExtensionType[] extensions = extensionTypes?.ToArray() ?? [];
            byte[] methodBuffer = new byte[1 + extensions.Length * 2];

            methodBuffer.WriteU8((byte)TokenProgramInstructions.Values.Reallocate, MethodOffset);

            for (int i = 0; i < extensions.Length; i++)
            {
                methodBuffer.WriteU16((ushort)extensions[i], 1 + i * 2);
            }

            return methodBuffer;
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeMint"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeMintData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Decimals", data.GetU8(1));
            decodedInstruction.Values.Add("Mint Authority", data.GetPubKey(2));

            var hasFreezeAuthority = data.GetBool(34);
            
            decodedInstruction.Values.Add("Freeze Authority Option", hasFreezeAuthority);
            if(hasFreezeAuthority)
                decodedInstruction.Values.Add("Freeze Authority", data.GetPubKey(35));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeMultiSignature"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeMultiSignatureData(DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            byte numSigners = data.GetU8(1);
            decodedInstruction.Values.Add("Required Signers", numSigners);
            for (int i = 2; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 1}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.Transfer"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeTransferData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Source", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Destination", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.Approve"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeApproveData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Source", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Delegate", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.Revoke"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeRevokeData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Source", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[1]]);
            for (int i = 2; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 1}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.SetAuthority"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSetAuthorityData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Current Authority", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority Type", Enum.Parse(typeof(AuthorityType), data.GetU8(1).ToString()));
            decodedInstruction.Values.Add("New Authority Option", data.GetU8(2));
            if (data.Length >= 34)
            {
                decodedInstruction.Values.Add("New Authority", data.GetPubKey(3));
            }
            for (int i = 2; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 1}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.MintTo"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeMintToData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Destination", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Mint Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.Burn"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeBurnData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.CloseAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeCloseAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Destination", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.FreezeAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeFreezeAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Freeze Authority", keys[keyIndices[2]]);
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.ThawAccount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeThawAccountData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Freeze Authority", keys[keyIndices[2]]);
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.TransferChecked"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeTransferCheckedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Source", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Destination", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Decimals", data.GetU8(9));
            for (int i = 4; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 3}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.ApproveChecked"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeApproveCheckedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Source", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Delegate", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[3]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Decimals", data.GetU8(9));
            for (int i = 4; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 3}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.MintToChecked"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeMintToCheckedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Destination", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Mint Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Decimals", data.GetU8(9));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.BurnChecked"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeBurnCheckedData(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", keys[keyIndices[2]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
            decodedInstruction.Values.Add("Decimals", data.GetU8(9));
            for (int i = 3; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 2}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.SyncNative"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeSyncNativeData(DecodedInstruction decodedInstruction, IList<PublicKey> keys,
            byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
        }

        /// <summary>
        /// Encodes the transaction instruction data for the methods which only require the amount.
        /// </summary>
        /// <param name="method">The method identifier.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeAmountLayout(byte method, ulong amount)
        {
            byte[] methodBuffer = new byte[9];

            methodBuffer.WriteU8(method, MethodOffset);
            methodBuffer.WriteU64(amount, 1);

            return methodBuffer;
        }

        /// <summary>
        /// Encodes the transaction instruction data for the methods which only require the amount and the number of decimals.
        /// </summary>
        /// <param name="method">The method identifier.</param>
        /// <param name="amount">The amount of tokens.</param>
        /// <param name="decimals">The decimals of the token.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeAmountCheckedLayout(byte method, ulong amount, byte decimals)
        {
            byte[] methodBuffer = new byte[10];

            methodBuffer.WriteU8(method, MethodOffset);
            methodBuffer.WriteU64(amount, 1);
            methodBuffer.WriteU8(decimals, 9);

            return methodBuffer;
        }


        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeAccount2"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeAccount2(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", data.GetPubKey(1));
        }


        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeAccount3"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeAccount3(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Mint", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Authority", data.GetPubKey(1));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeMultiSignature2"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeMultiSignature2(DecodedInstruction decodedInstruction,
            ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            byte numSigners = data.GetU8(1);
            decodedInstruction.Values.Add("Required Signers", numSigners);
            for (int i = 1; i < keyIndices.Length; i++)
            {
                decodedInstruction.Values.Add($"Signer {i - 1}", keys[keyIndices[i]]);
            }
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.InitializeMint2"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeMint2(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Decimals", data.GetU8(1));
            decodedInstruction.Values.Add("Mint Authority", data.GetPubKey(2));

            var hasFreezeAuthority = data.GetBool(34);

            decodedInstruction.Values.Add("Freeze Authority Option", hasFreezeAuthority);
            if (hasFreezeAuthority)
                decodedInstruction.Values.Add("Freeze Authority", data.GetPubKey(35));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.AmountToUiAmount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeAmountToUiAmount(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Amount", data.GetU64(1));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.UiAmountToAmount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeUiAmountToAmount(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Amount", Encoding.UTF8.GetString(data[1..]));
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.UiAmountToAmount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeGetAccountDataSize(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            List<Token2022ExtensionType> extensions = [];

            for (int offset = 1; offset + 1 < data.Length; offset += 2)
            {
                ushort value = data.GetU16(offset);
                if (Enum.IsDefined(typeof(Token2022ExtensionType), value))
                {
                    extensions.Add((Token2022ExtensionType)value);
                }
            }

            decodedInstruction.Values.Add("Extension Types", extensions);
        }

        /// <summary>
        /// Decodes the instruction instruction data  for the <see cref="TokenProgramInstructions.Values.UiAmountToAmount"/> method
        /// </summary>
        /// <param name="decodedInstruction">The decoded instruction to add data to.</param>
        /// <param name="data">The instruction data to decode.</param>
        /// <param name="keys">The account keys present in the transaction.</param>
        /// <param name="keyIndices">The indices of the account keys for the instruction as they appear in the transaction.</param>
        internal static void DecodeInitializeImmutableOwner(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
        }

        /// <summary>
        /// Decodes the instruction data for the <see cref="TokenProgramInstructions.Values.InitializeMintCloseAuthority"/> method.
        /// </summary>
        internal static void DecodeInitializeMintCloseAuthority(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Mint", keys[keyIndices[0]]);
            bool hasCloseAuthority = data.Length > 1 && data.GetU8(1) == 1;
            decodedInstruction.Values.Add("Close Authority Option", hasCloseAuthority);
            if (hasCloseAuthority)
            {
                decodedInstruction.Values.Add("Close Authority", data.GetPubKey(2));
            }
        }

        /// <summary>
        /// Decodes the instruction data for the <see cref="TokenProgramInstructions.Values.Reallocate"/> method.
        /// </summary>
        internal static void DecodeReallocate(DecodedInstruction decodedInstruction, ReadOnlySpan<byte> data,
            IList<PublicKey> keys, byte[] keyIndices)
        {
            decodedInstruction.Values.Add("Account", keys[keyIndices[0]]);
            decodedInstruction.Values.Add("Payer", keys[keyIndices[1]]);
            decodedInstruction.Values.Add("Owner", keys[keyIndices[3]]);

            List<Token2022ExtensionType> extensions = [];
            for (int offset = 1; offset + 1 < data.Length; offset += 2)
            {
                ushort value = data.GetU16(offset);
                if (Enum.IsDefined(typeof(Token2022ExtensionType), value))
                {
                    extensions.Add((Token2022ExtensionType)value);
                }
            }

            decodedInstruction.Values.Add("Extension Types", extensions);
        }

        /// <summary>
        /// Encodes the transaction instruction data for the methods that require only an owner public key.
        /// </summary>
        /// <param name="method">The method identifier.</param>
        /// <param name="owner">The owner public key.</param>
        /// <returns>The byte array with the encoded data.</returns>
        private static byte[] EncodeInitializeAccountOwnerData(byte method, PublicKey owner)
        {
            byte[] methodBuffer = new byte[33];

            methodBuffer.WriteU8(method, MethodOffset);
            methodBuffer.WritePubKey(owner, 1);

            return methodBuffer;
        }
    }
}