# Token-2022 extensions

Token-2022 uses `Token2022Program.ProgramIdKey`, not the original Token program id. All amounts and fee caps below are raw token units. Account addresses and mint addresses remain public when using confidential balances; balances and transfer amounts are encrypted.

```csharp
using Solnet.Programs;
using Solnet.Programs.Models.Token2022;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using AccountState = Solnet.Programs.Models.TokenProgram.TokenAccount.AccountState;
```

## Supported operations

| Extension | Instruction builders |
| --- | --- |
| Transfer fees | `InitializeTransferFeeConfig`, `SetTransferFee`, `TransferCheckedWithFee`, `HarvestWithheldTokensToMint`, `WithdrawWithheldTokensFromMint`, `WithdrawWithheldTokensFromAccounts` |
| Transfer hooks | `InitializeTransferHook`, `UpdateTransferHook`, `TransferCheckedWithTransferHook`, `DeriveTransferHookValidationAddress` |
| Permanent delegate | `InitializePermanentDelegate`; existing token transfer, burn, and authority-management instructions apply |
| Metadata pointer | `InitializeMetadataPointer`, `UpdateMetadataPointer` |
| Default account state | `InitializeDefaultAccountState`, `UpdateDefaultAccountState` |
| Non-transferable tokens | `InitializeNonTransferableMint` |
| Confidential balances | Mint initialize/update; account configure/approve/empty; deposit, apply pending balance, transfer, withdraw; credit controls; registry-based configuration |
| Confidential transfer fees | Mint configuration; confidential transfers with fees; harvest, withdraw, and harvest-policy controls |
| Proof verification | `ZkElGamalProofProgram.VerifyProof`, `VerifyProofFromAccount`, `CloseContextState` |

## Allocation and initialization order

Choose mint extensions before creating the mint. Initialize each extension after `SystemProgram.CreateAccount` and before `Token2022Program.InitializeMint2`, in the same transaction. Most mint extensions cannot be added to an already initialized mint. `Reallocate` applies to token accounts, not to adding arbitrary mint extensions.

The allocation helpers account for the base layout, mint padding, account-type byte, four-byte TLV headers, deduplication, and the special multisig-size adjustment. They support the fixed-size extensions described here, plus mint close authority, interest-bearing configuration, memo transfer, immutable owner, and CPI guard. Other types, including variable-size token metadata, are rejected rather than assigned guessed lengths.

This example creates a fee-bearing mint. Use your existing funded payer and a new mint keypair:

```csharp
static byte[] BuildFeeMintTransaction(IRpcClient rpc, Account payer, Account mint)
{
    var extensions = new[] { Token2022ExtensionType.TransferFeeConfig };
    int space = Token2022Program.GetMintAccountDataSize(extensions);
    var rent = rpc.GetMinimumBalanceForRentExemption(space);
    var blockhash = rpc.GetLatestBlockHash();
    if (!rent.WasSuccessful || !blockhash.WasSuccessful)
        throw new InvalidOperationException("Unable to obtain rent or blockhash.");

    return new TransactionBuilder()
        .SetFeePayer(payer.PublicKey)
        .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
        .AddInstruction(SystemProgram.CreateAccount(
            payer.PublicKey, mint.PublicKey, rent.Result, (ulong)space,
            Token2022Program.ProgramIdKey))
        .AddInstruction(Token2022Program.InitializeTransferFeeConfig(
            mint.PublicKey, payer.PublicKey, payer.PublicKey,
            basisPoints: 100, maximumFee: 1_000_000))
        .AddInstruction(Token2022Program.InitializeMint2(
            mint.PublicKey, 6, payer.PublicKey))
        .Build(new List<Account> { payer, mint });

}
// Call with your RPC client, funded payer, and a new mint account; submit through your normal transaction flow.
```

For token accounts, combine `GetRequiredAccountExtensions(mintExtensions)` with explicitly requested account extensions, then call `GetTokenAccountDataSize`. Confidential balances are opt-in: add `ConfidentialTransferAccount` for accounts that use them, and `ConfidentialTransferFeeAmount` when confidential fees are needed. Allocate sufficient rent before configuring or reallocating an account. Initialize the base account first, then configure its confidential extension. Associated token accounts use the existing associated-account builders with the Token-2022 program id.

Combining `TransferFeeConfig` and `ConfidentialTransferMint` requires `ConfidentialTransferFeeConfig`. The helper rejects that combination if the confidential-fee configuration is missing. Non-transferable confidential mints also require the newer confidential mint/burn extension; that combination is outside these fixed-size allocation helpers.

## Fees and account state

Fetch account data with Base64 encoding and verify its owner before decoding:

```csharp
var response = rpc.GetAccountInfo(mintAddress);
if (!response.WasSuccessful || response.Result.Value == null)
    throw new InvalidOperationException("Mint account was not found.");
if (response.Result.Value.Owner != Token2022Program.ProgramIdKey.Key)
    throw new InvalidOperationException("Account is not owned by Token-2022.");

var mintState = Token2022Mint.Deserialize(
    Convert.FromBase64String(response.Result.Value.Data[0]));
var epochResponse = rpc.GetEpochInfo();
if (!epochResponse.WasSuccessful)
    throw new InvalidOperationException("Unable to obtain the current epoch.");
var activeFee = mintState.TransferFees?.GetFeeForEpoch(epochResponse.Result.Epoch);
ulong expectedFee = activeFee?.CalculateFee(amount) ?? 0;

var transfer = Token2022Program.TransferCheckedWithFee(
    source, mintAddress, destination, owner,
    amount, decimals: 6, expectedFee: expectedFee);
```

`CalculateFee` rounds up the percentage fee and applies its maximum, using integer arithmetic. Fee updates follow Token-2022's epoch scheduling rules; fetch current state again when submitting near an epoch transition. Fees are withheld on recipient token accounts. Harvest them into the mint, or withdraw them directly from token accounts with the configured withdraw authority.

`Token2022Account.Deserialize` exposes base account fields, withheld public fees, confidential state and counters, encrypted withheld fees, immutable-owner/non-transferable flags, and the transfer-hook flag. `Token2022Mint.Deserialize` exposes fee schedules, hook settings, metadata pointer, permanent delegate, default state, and confidential policies. Both readers preserve raw payloads of unknown future TLV types in `Extensions` and reject truncated or duplicate entries.

## Transfer hooks

Initialize the hook authority and hook program before initializing the mint:

```csharp
var initialize = Token2022Program.InitializeTransferHook(mint, hookAuthority, hookProgram);
PublicKey validationAddress = Token2022Program.DeriveTransferHookValidationAddress(mint, hookProgram);
var update = Token2022Program.UpdateTransferHook(mint, hookAuthority, newHookProgram);
```

Hook transfers require additional accounts. Fetch the hook's ExtraAccountMetaList and resolve its dynamic PDA/account references using a resolver compatible with that hook. Pass the complete resolved extra-account sequence, including the hook program and validation account, in the order required by Token-2022 and the hook. Preserve each account's signer and writable flags. Solnet does not automatically fetch or resolve arbitrary hook account schemas.

```csharp
var transfer = Token2022Program.TransferCheckedWithTransferHook(
    source, mint, destination, owner, amount, decimals: 6,
    resolvedHookAccounts: resolvedAccounts);
```

For a mint with both fees and a hook, use `TransferCheckedWithFee` and its `remainingAccounts` parameter. The hook program is a separate deployed program; these builders configure and invoke it, rather than deploying hook logic.

## Delegate, metadata, default state, and non-transferability

These configuration instructions run before mint initialization:

```csharp
var delegateInstruction = Token2022Program.InitializePermanentDelegate(mint, delegateAuthority);
var metadataInstruction = Token2022Program.InitializeMetadataPointer(mint, metadataAuthority, metadataAccount);
var defaultStateInstruction = Token2022Program.InitializeDefaultAccountState(mint, AccountState.Frozen);
var nonTransferableInstruction = Token2022Program.InitializeNonTransferableMint(mint);
```

Select the corresponding mint extensions when calculating allocation. A frozen default requires a freeze authority on the mint; that authority can later use `UpdateDefaultAccountState` or thaw individual accounts. Non-transferability is permanent. The permanent delegate can operate on public balances under Token-2022's rules; it does not grant decryption access to confidential balances.

`InitializeMetadataPointer` configures an address. It does not create or populate token metadata. Initialize the metadata account with its owning metadata program; pointing at the mint itself also requires allocating and initializing the variable-size token-metadata extension separately.

Optional extension authorities or target addresses use `null` to encode an absent key. For zero-sentinel fields (such as hook and metadata-pointer keys), the all-zero public key is rejected to avoid silently interpreting a supplied key as absence. Transfer-fee configuration uses the program's tagged optional-key encoding instead. Instructions with an authority accept optional multisig signer public keys; an empty or omitted signer list means the authority itself signs. For multisigs, provide the multisig authority and its actual signing members, then sign the transaction with those members.

## Confidential balance workflow

Confidential transfers require compatible ElGamal keys, authenticated-encryption keys, encrypted balances, and zero-knowledge proofs. These builders **do not generate keys for this protocol, encrypt/decrypt balances, or generate proofs**. Supply those values from Solana's `spl-token-confidential-transfer-proof-generation`/ZK SDK or a compatible integration. A wallet signing key is not an ElGamal public key. Never use zero-filled placeholder ciphertexts or proofs as a substitute for generating valid values.

1. Allocate and initialize the mint's `ConfidentialTransferMint` extension, then initialize the mint.
2. Allocate token accounts with the confidential extension, initialize their base account state, and configure them using a PubkeyValidity proof and a correctly encrypted zero balance. Approve accounts if the mint uses manual approval.
3. Deposit public tokens with `DepositConfidentialTokens` (at most `2^48 - 1` raw units per deposit).
4. Read fresh confidential state and use the encryption integration to compute the new decryptable balance; apply pending credits with `ApplyConfidentialPendingBalance`.
5. Generate transfer proofs and the new source decryptable balance. Verify proofs either inside the transaction or into proof context accounts, then call `TransferConfidentialTokens`.
6. Apply pending credits on the recipient. Use equality and U64 range proofs with `WithdrawConfidentialTokens` to withdraw back into the public balance.
7. Before closing the token account, clear pending balances and withheld fees, then use `EmptyConfidentialTransferAccount` with a zero-ciphertext proof and the normal close-account instruction.

### Proof references

`ConfidentialProofLocation.FromInstructionOffset(offset)` uses a nonzero signed-byte offset relative to the **token instruction**. The builder includes the instructions sysvar automatically. It does not append proof verification instructions for you; keep offsets correct if other instructions are inserted.

This example references a PubkeyValidity verifier immediately after the configure instruction. Files contain real values generated by your encryption/proof integration:

```csharp
byte[] encryptedZero = File.ReadAllBytes("encrypted-zero-balance.bin"); // 36 bytes
byte[] proofData = File.ReadAllBytes("pubkey-validity-proof-data.bin"); // serialized context + proof

var configure = Token2022Program.ConfigureConfidentialTransferAccount(
    tokenAccount, mint, owner, encryptedZero,
    ConfidentialProofLocation.FromInstructionOffset(1));
var verify = ZkElGamalProofProgram.VerifyProof(
    ZkElGamalProofProgram.ProofType.PubkeyValidity, proofData);
// Add configure, then verify, to the same transaction in this order.
```

`ConfidentialProofLocation.FromContextStateAccount(account)` references a previously verified proof context. The builder encodes a zero offset and includes the account. For a confidential transfer, supply equality, batched three-handle ciphertext-validity, and U128 range contexts in that order:

```csharp
var transfer = Token2022Program.TransferConfidentialTokens(
    source, mint, destination, owner,
    newSourceDecryptableAvailableBalance: encryptedNewBalance,
    equalityProof: ConfidentialProofLocation.FromContextStateAccount(equalityContext),
    ciphertextValidityProof: ConfidentialProofLocation.FromContextStateAccount(validityContext),
    rangeProof: ConfidentialProofLocation.FromContextStateAccount(rangeContext));
```

If the mint has an auditor, supply both 64-byte auditor ciphertext components generated with that auditor key. Null auditor ciphertexts encode zeros and are appropriate only when the mint has no auditor. Retain the auditor secret keys needed for historical audit access when rotating keys.

Proof instructions may exceed transaction limits. Use `VerifyProofFromAccount` for proofs prewritten to a separate account, or verify proofs across transactions into appropriately allocated, rent-exempt context accounts owned by the ZK ElGamal proof program. Allocate the official context-state size for the proof type using your proof integration. Supply both `contextStateAccount` and `contextStateAuthority` to persist a verified context. The context authority need not sign verification, but must sign `CloseContextState` to reclaim rent. The cluster must have the ZK ElGamal proof program enabled.

### Confidential fees

A confidential fee mint also initializes `ConfidentialTransferFeeConfig` with the withdraw authority's ElGamal public key. Use `TransferConfidentialTokensWithFee` with equality, transfer validity, percentage-with-cap, fee validity, and U256 range proofs. `HarvestConfidentialWithheldTokensToMint` and the two `WithdrawConfidentialWithheldTokens...` builders collect encrypted fees. Withdrawal requires a CiphertextCiphertextEquality proof and the new decryptable destination balance. `EnableConfidentialFeeHarvestToMint` and `DisableConfidentialFeeHarvestToMint` control permissionless harvesting.

## Reference and validation

Instruction discriminators, account ordering, data layouts, and TLV sizes are based on the [official Token-2022 interface at commit 6704f8b](https://github.com/solana-program/token-2022/tree/6704f8b103d19059c1feeed3bf6c589a9edfd9c3/interface/src). Proof instructions follow the [native proof-program interface](https://github.com/solana-program/zk-elgamal-proof/tree/main/interface/src).

See Solana's [confidential balance guide](https://solana.com/docs/tokens/extensions/confidential-transfer), [extension guide](https://www.solana-program.com/docs/token-2022/extensions), and [project status](https://www.solana-program.com/docs/token-2022/status) for protocol behavior and cluster requirements. Solnet validates framing and fixed-size inputs locally; Token-2022 and the proof program perform on-chain authorization and proof verification.
