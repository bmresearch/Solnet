using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Programs
{
    public static partial class Token2022Program
    {
        private static DecodedInstruction DecodeExtensionInstruction(ReadOnlySpan<byte> data, IList<PublicKey> keys, byte[] indices)
        {
            string[] names = data[0] switch
            {
                26 => new[] { "InitializeTransferFeeConfig", "TransferCheckedWithFee", "WithdrawWithheldTokensFromMint", "WithdrawWithheldTokensFromAccounts", "HarvestWithheldTokensToMint", "SetTransferFee" },
                27 => new[] { "InitializeConfidentialTransferMint", "UpdateConfidentialTransferMint", "ConfigureConfidentialTransferAccount", "ApproveConfidentialTransferAccount", "EmptyConfidentialTransferAccount", "DepositConfidentialTokens", "WithdrawConfidentialTokens", "TransferConfidentialTokens", "ApplyConfidentialPendingBalance", "EnableConfidentialCredits", "DisableConfidentialCredits", "EnableNonConfidentialCredits", "DisableNonConfidentialCredits", "TransferConfidentialTokensWithFee", "ConfigureConfidentialTransferAccountWithRegistry" },
                28 => new[] { "InitializeDefaultAccountState", "UpdateDefaultAccountState" },
                36 => new[] { "InitializeTransferHook", "UpdateTransferHook" },
                37 => new[] { "InitializeConfidentialTransferFeeConfig", "WithdrawConfidentialWithheldTokensFromMint", "WithdrawConfidentialWithheldTokensFromAccounts", "HarvestConfidentialWithheldTokensToMint", "EnableConfidentialFeeHarvestToMint", "DisableConfidentialFeeHarvestToMint" },
                39 => new[] { "InitializeMetadataPointer", "UpdateMetadataPointer" },
                _ => null
            };
            string name;
            int payloadOffset;
            if (data[0] == 32 || data[0] == 35)
            {
                name = data[0] == 32 ? "InitializeNonTransferableMint" : "InitializePermanentDelegate";
                payloadOffset = 1;
            }
            else
            {
                if (names == null || data.Length < 2 || data[1] >= names.Length) return null;
                name = names[data[1]];
                payloadOffset = 2;
            }
            ArgumentNullException.ThrowIfNull(keys);
            ArgumentNullException.ThrowIfNull(indices);
            var accounts = new List<PublicKey>();
            foreach (byte index in indices)
            {
                if (index >= keys.Count) throw new ArgumentException("Account index is outside the transaction account list.", nameof(indices));
                accounts.Add(keys[index]);
            }
            return new DecodedInstruction
            {
                PublicKey = ProgramIdKey,
                ProgramName = ProgramName,
                InstructionName = name,
                Values = new Dictionary<string, object> { ["Accounts"] = accounts, ["Extension Data"] = data[payloadOffset..].ToArray() },
                InnerInstructions = new List<DecodedInstruction>()
            };
        }
    }
}
