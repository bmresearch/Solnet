<p align="center">
    <img src="https://raw.githubusercontent.com/bmresearch/Solnet/master/assets/icon.png" margin="auto" height="175"/>
</p>

<p align="center">
    <a href="https://github.com/bmresearch/Solnet/actions/workflows/dotnet.yml">
        <img src="https://github.com/bmresearch/Solnet/actions/workflows/dotnet.yml/badge.svg"
            alt="Build" ></a>
<br/>
    <a href="">
        <img src="https://img.shields.io/github/license/bmresearch/Solnet?style=flat-square"
            alt="Code License"></a>
    <a href="https://twitter.com/intent/follow?screen_name=blockmountainio">
        <img src="https://img.shields.io/twitter/follow/blockmountainio?style=flat-square&logo=twitter"
            alt="Follow on Twitter"></a>
    <a href="https://discord.gg/cReXaBReZt">
       <img alt="Discord" src="https://img.shields.io/discord/849407317761064961?style=flat-square"
            alt="Join the discussion!"></a>
</p>

<div style="text-align:center">

<p>

# Introduction

Solnet is Solana's C# SDK designed to integrate seamlessly with the .NET ecosystem for web, mobile, and desktop apps. Whether you're a seasoned developer or just getting started, you'll find examples, docs, and APIs that make Solana development in .NET straightforward.

</p>

</div>


## Features
- Full JSON RPC API coverage
- Full Streaming JSON RPC API coverage
- Wallet and accounts (Phantom and solana-keygen compatible)
- Keystore (Phantom and solana-keygen compatible)
- Transaction decoding/encoding (base64 and wire format)
- Message decoding/encoding (base64 and wire format)
- Instruction decompilation
- TokenWallet object to send SPL tokens and JIT provisioning of [Associated Token Accounts](https://spl.solana.com/associated-token-account)
- Programs
    - Native Programs
      - System Program
      - Stake Program
    - Solana Program Library (SPL)
      - Compute Budget Program
      - Account Compression Program
      - Governance Program
      - StakePool Program
      - Address Lookup Table Program
      - Memo Program
      - Token Program
      - Token Swap Program
      - Associated Token Account Program
      - Name Service Program
      - Shared Memory Program

For maintainability and due to the size/complexity of some other programs, this repository focuses on Solana's Native Programs and SPL programs. For other commonly used programs see:

- [Serum](https://github.com/bmresearch/Solnet.Serum/)
- [Mango](https://github.com/bmresearch/Solnet.Mango/)
- [Pyth](https://github.com/bmresearch/Solnet.Pyth/)

Maintained by Bifrost <img src="https://avatars.githubusercontent.com/u/119550733?s=64&v=4" width=25 />
- [Metaplex](https://github.com/bmresearch/Solnet.Metaplex/)
- [Raydium](https://github.com/Bifrost-Technologies/Solnet.Raydium/)
- [JupiterSwap](https://github.com/Bifrost-Technologies/Solnet.JupiterSwap)
- [JupiterPerps](https://github.com/Bifrost-Technologies/Solnet.JupiterPerps)
- [Moonshot](https://github.com/Bifrost-Technologies/Solnet.Moonshot)
- [Pumpfun](https://github.com/Bifrost-Technologies/Solnet.Pumpfun)
- [Ore](https://github.com/Bifrost-Technologies/Solnet.Ore)

## Requirements
- .NET 6.0 (deprecated)
- .NET 8.0+ (recommended)

## Dependencies
- BifrostSecurity
- Portable.BouncyCastle (optional)

## Installation

Use the .NET CLI to install Solnet packages into your project (choose the ones you need):

```powershell
# Rpc client and streaming
 dotnet add package Solana.Rpc
 dotnet add package Solana.Programs

# Wallets and keys
 dotnet add package Solana.Wallet

# Token helpers
 dotnet add package Solana.Extensions

# Keystore utilities
 dotnet add package Solana.Keystore
```

Latest Packages on NuGet:
- [Solana.Rpc](https://www.nuget.org/packages/Solana.Rpc/)
- [Solana.Wallet](https://www.nuget.org/packages/Solana.Wallet/)
- [Solana.Programs](https://www.nuget.org/packages/Solana.Programs/)
- [Solana.Extensions](https://www.nuget.org/packages/Solana.Extensions/)
- [Solana.Keystore](https://www.nuget.org/packages/Solana.Keystore/)

## Quickstart

A minimal example to fetch a balance and send a memo:

```csharp
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Types;
using Solnet.Programs;
using Solnet.Wallet;

var rpc = ClientFactory.GetClient(Cluster.MainNet);
var wallet = new Wallet();
var from = wallet.GetAccount(0);

// Get balance
var bal = rpc.GetBalance(from.PublicKey);
Console.WriteLine($"Balance: {bal.Result.Value} lamports");

// Send a simple memo transaction
var blockhash = rpc.GetLatestBlockHash();
var tx = new TransactionBuilder()
    .SetRecentBlockHash(blockhash.Result.Value.Blockhash)
    .SetFeePayer(from)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(MemoProgram.NewMemo(from, "Hello from Solnet"))
    .Build(from);

var sig = rpc.SendTransaction(tx);
Console.WriteLine($"tx: {sig.Result}");
```

## Examples

The [`Solnet.Examples`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Examples/) project contains runnable code examples. Some examples derive accounts from a seed and may fail if an account at a derivation index already exists with the same seed. If you see simulation logs like `account address is ... already in use`, increment the derivation index (e.g., `wallet.GetAccount(index + 1)`).

### Wallets

The [`Solnet.Wallet`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Wallet/) project implements wallet and key generation compatible with both `solana-keygen` and [Phantom](https://phantom.app/).

#### Initialize a keypair from a secret key
```csharp
var account = Account.FromSecretKey("");
```

#### Initialize a wallet (Phantom-compatible by default)
```csharp
var wallet = new Wallet("mnemonic words ...", WordList.English);
var account = wallet.GetAccount(10);
```

#### Initialize a wallet compatible with solana-keygen
```csharp
var wallet = new Wallet("mnemonic words ...", WordList.English, "passphrase", SeedMode.Bip39);
var account = wallet.Account; // solana-keygen uses a fixed derivation path
```

### Generating new wallets
```csharp
var newMnemonic = new Mnemonic(WordList.English, WordCount.Twelve);
var wallet = new Wallet(newMnemonic);
```

### KeyStore

The [`Solnet.KeyStore`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.KeyStore/) project enables secure storage of keys, seeds, and mnemonics. It implements the [Web3 Secret Storage Definition](https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition) and includes `SolanaKeyStoreService` to read keys generated by `solana-keygen`.

#### Secret KeyStore Service
```csharp
var secretKeyStoreService = new SecretKeyStoreService();
var jsonString = secretKeyStoreService.EncryptAndGenerateDefaultKeyStoreAsJson(password, data, address);

try
{
    var decrypted = KeyStore.DecryptKeyStoreFromJson(password, jsonString);
}
catch (Exception)
{
    Console.WriteLine("Invalid password!");
}
```

#### Solana KeyStore Service
```csharp
var solanaKeyStoreService = new SolanaKeyStoreService();
var wallet = solanaKeyStoreService.RestoreKeystore(filePath, passphrase);
```

### RPC and Streaming RPC

The [`Solnet.Rpc`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Rpc/) project contains a full implementation of the [Solana JSON RPC](https://docs.solana.com/developing/clients/jsonrpc-api).

#### ClientFactory pattern
```csharp
var rpcClient = ClientFactory.GetClient(Cluster.MainNet, logger);
var paidClient = ClientFactory.GetClient("https://your.paid.rpc", logger);
var streamingRpcClient = ClientFactory.GetStreamingClient(Cluster.MainNet, logger);
```

#### Using the RPC
```csharp
var accountInfo = rpcClient.GetAccountInfo("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj");
var tokenAccounts = rpcClient.GetTokenAccountsByOwner("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj");
var wrappedSolAccounts = rpcClient.GetTokenAccountsByOwner(
    "5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj",
    "So11111111111111111111111111111111111111112");

var serumAddress = "9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin";
var programAccounts = rpcClient.GetProgramAccounts(serumAddress);
var filters = new List<MemCmp>() { new MemCmp { Offset = 45, Bytes = OwnerAddress } };
var filteredProgramAccounts = rpcClient.GetProgramAccounts(serumAddress, memCmpList: filters);
```

#### Using the Streaming RPC
```csharp
var txSig = rpcClient.SendTransaction(tx);
var subscription = streamingRpcClient.SubscribeSignature(txSig.Result, (state, response) =>
{
    // handle confirmation
}, Commitment.Finalized);
```

### Sending a transaction

#### Important: Understanding priority fees
Poorly optimized transactions can get dropped due to high compute demand. Always specify both compute unit limit and price to improve inclusion probability.

```csharp
var txBuilder = new TransactionBuilder()
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000));
```

```csharp
var rpcClient = ClientFactory.GetClient(Cluster.MainNet);
var wallet = new Wallet();
var fromAccount = wallet.GetAccount(0);
var toAccount = wallet.GetAccount(1);
var blockHash = rpcClient.GetLatestBlockHash();

var tx = new TransactionBuilder()
    .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
    .SetFeePayer(fromAccount)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)"))
    .AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 100000))
    .Build(fromAccount);

var firstSig = rpcClient.SendTransaction(tx);
```

### Create, Initialize and Mint
```csharp
var wallet = new Wallet(MnemonicWords);

var blockHash = rpcClient.GetLatestBlockHash();
var minBalanceForExemptionAcc = rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
var minBalanceForExemptionMint = rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;

var mintAccount = wallet.GetAccount(21);
var ownerAccount = wallet.GetAccount(10);
var initialAccount = wallet.GetAccount(22);

var tx = new TransactionBuilder()
    .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
    .SetFeePayer(ownerAccount)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        mintAccount,
        minBalanceForExemptionMint,
        TokenProgram.MintAccountDataSize,
        TokenProgram.ProgramIdKey))
    .AddInstruction(TokenProgram.InitializeMint(
        mintAccount.PublicKey,
        2,
        ownerAccount.PublicKey,
        ownerAccount.PublicKey))
    .AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        initialAccount,
        minBalanceForExemptionAcc,
        TokenProgram.TokenAccountDataSize,
        TokenProgram.ProgramIdKey))
    .AddInstruction(TokenProgram.InitializeAccount(
        initialAccount.PublicKey,
        mintAccount.PublicKey,
        ownerAccount.PublicKey))
    .AddInstruction(TokenProgram.MintTo(
        mintAccount.PublicKey,
        initialAccount.PublicKey,
        25000,
        ownerAccount))
    .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
    .Build(new List<Account>{ ownerAccount, mintAccount, initialAccount });
```

### Transfer a Token to a new Token Account
```csharp
var rpcClient = ClientFactory.GetClient(Cluster.MainNet);
var wallet = new Wallet();

var blockHash = rpcClient.GetLatestBlockHash();
var minBalanceForExemptionAcc =
    rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;

var mintAccount = wallet.GetAccount(21);
var ownerAccount = wallet.GetAccount(10);
var initialAccount = wallet.GetAccount(22);
var newAccount = wallet.GetAccount(23);

var tx = new TransactionBuilder()
    .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
    .SetFeePayer(ownerAccount)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        newAccount,
        minBalanceForExemptionAcc,
        TokenProgram.TokenAccountDataSize,
        TokenProgram.ProgramIdKey))
    .AddInstruction(TokenProgram.InitializeAccount(
        newAccount.PublicKey,
        mintAccount.PublicKey,
        ownerAccount.PublicKey))
    .AddInstruction(TokenProgram.Transfer(
        initialAccount.PublicKey,
        newAccount.PublicKey,
        25000,
        ownerAccount))
    .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
    .Build(new List<Account>{ ownerAccount, newAccount });
```

### Transaction and Message decoding
```csharp
var tx = Transaction.Deserialize(txData);
var msg = Message.Deserialize(msgData);
var signedTx = Transaction.Populate(msg, new List<byte[]> { account.Sign(msgData) });
```

### Programs

The [`Solnet.Programs`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Programs/) project contains implementations of several Native and SPL programs. See [`Solnet.Examples`](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Examples/) for more depth, including multi-signature operations.

### Hello Solana World
```csharp
var memoInstruction = MemoProgram.NewMemo(wallet.Account, "Hello Solana World, using Solnet :)");

var recentHash = rpcClient.GetLatestBlockHash();

var tx = new TransactionBuilder()
    .SetFeePayer(wallet.Account)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(memoInstruction)
    .SetRecentBlockHash(recentHash.Result.Value.Blockhash)
    .Build(wallet.Account);
```

### Creating and sending tokens to an Associated Token Account
```csharp
var recentHash = rpcClient.GetLatestBlockHash();

PublicKey associatedTokenAccountOwner = new ("65EoWs57dkMEWbK4TJkPDM76rnbumq7r3fiZJnxggj2G");
PublicKey associatedTokenAccount =
    AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(associatedTokenAccountOwner, mintAccount);

byte[] txBytes = new TransactionBuilder()
    .SetRecentBlockHash(recentHash.Result.Value.Blockhash)
    .SetFeePayer(ownerAccount)
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitLimit(30000))
    .AddInstruction(ComputeBudgetProgram.SetComputeUnitPrice(1000000))
    .AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
        ownerAccount,
        associatedTokenAccountOwner,
        mintAccount))
    .AddInstruction(TokenProgram.Transfer(
        initialAccount,
        associatedTokenAccount,
        25000,
        ownerAccount))
    .AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net"))
    .Build(new List<Account> { ownerAccount });

string signature = rpcClient.SendTransaction(txBytes);
```

### Instruction decoding
```csharp
var msg = Message.Deserialize(msgBase64);
var decodedInstructions = InstructionDecoder.DecodeInstructions(msg);
```

## Contribution

We encourage everyone to contribute, submit issues and PRs, and join discussions. Every kind of help is welcome.

## Legacy Maintainers

* **Hugo** - [murlokito](https://github.com/murlokito)
* **Tiago** - [tiago](https://github.com/tiago18c)

## Current Maintainers

* **Nathan** - [BifrostTitan](https://github.com/BifrostTitan)

See also the list of [contributors](https://github.com/bmresearch/Solnet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
