<p align="center">
    <img src="https://raw.githubusercontent.com/bmresearch/Solnet/master/assets/icon.png" margin="auto" height="175"/>
</p>

<div align="center">
    <a href="https://dev.azure.com/bmresearch/solnet/_build/latest?definitionId=2&branchName=refs%2Fpull%2F7%2Fmerge">
        <img src="https://img.shields.io/azure-devops/build/bmresearch/solnet/2/master?style=flat-square"
            alt="Azure DevOps Build Status (master)" ></a>
    <a href="https://img.shields.io/azure-devops/coverage/bmresearch/solnet/2/master">
        <img src="https://img.shields.io/azure-devops/coverage/bmresearch/solnet/2/master?style=flat-square"
            alt="Azure DevOps Cobertura Code Coverage (master)"></a>
    <a href="">
        <img src="https://img.shields.io/github/license/bmresearch/solnet?style=flat-square"
            alt="Code License"></a>
    <a href="https://twitter.com/intent/follow?screen_name=blockmountainio">
        <img src="https://img.shields.io/twitter/follow/blockmountainio?style=flat-square&logo=twitter"
            alt="Follow on Twitter"></a>
    <a href="https://discord.gg/YHMbpuS3Tx">
        <img alt="Discord" src="https://img.shields.io/discord/849407317761064961?style=flat-square"
            alt="Join the discussion!"></a>
</div>

<div style="text-align:center">

<p>

# Introduction

Solnet is Solana's .NET SDK to integrate with the .NET ecosystem. Wherever you are developing for the Web or Desktop, we are here to help. Learn more about the provided samples, documentation, integrating the SDK into your app, and more [here](https://blockmountain.io/Solnet/).

</p>

</div>


## Features
- Full JSON RPC API coverage
- Full Streaming JSON RPC API coverage
- Wallet and accounts (sollet and solana-keygen compatible)
- Keystore (sollet and solana-keygen compatible)
- Programs
    - Native Programs
      - System Program
    - Solana Program Library (SPL)
      - Memo Program
      - Token Program
      - Associated Token Account Program
      - Name Service Program
      - Shared Memory Program

For the sake of maintainability and sometimes due to the size and complexity of some other programs, this repository will only contain Solana's Native Programs and Programs who are part of the SPL,
for a list of other commonly needed programs see below:

- [Serum](https://github.com/bmresearch/Solnet.Serum/)

## Requirements
- net 5.0

## Dependencies
- Chaos.NaCl.Standard
- Portable.BouncyCastle

## Examples

The [Solnet.Examples](https://github.com/bmresearch/Solnet/src/Solnet.Examples/) project contains some code examples, but essentially we're trying very hard to
make it intuitive and easy to use the library.

### Initializing both wallets from Sollet and solana-keygen

```c#
// To initialize a wallet and have access to the same keys generated in solana-keygen
var wallet = new Wallet("mnemonic words ...", Wordlist.English, "passphrase", SeedMode.Bip39);

// To initialize a wallet and have access to the same keys generated in sollet (the default)
var sollet = new Wallet("mnemonic words ...", Wordlist.English);

// Retrieve accounts by derivation path index
var account = sollet.GetAccount(10);


// Restore by mnemonic
var mnemonic = new Mnemonic("mnemonic words ...");
var wallet = new Wallet(mnemonic);

// Generate a new mnemonic
var mnemonic2 = new Mnemonic(WordList.English, WordCount.TwentyFour);
``` 

### Sending a transaction

```c#
// Initialize the rpc client and a wallet
var rpcClient = ClientFactory.GetClient(Cluster.MainNet);
var wallet = new Wallet();
// Get the source account
var fromAccount = wallet.GetAccount(0);
// Get the destination account
var toAccount = wallet.GetAccount(1);
// Get a recent block hash to include in the transaction
var blockHash = rpcClient.GetRecentBlockHash();

// Initialize a transaction builder and chain as many instructions as you want before building the message
var tx = new TransactionBuilder().
        SetRecentBlockHash(blockHash.Result.Value.Blockhash).
        SetFeePayer(fromAccount).
        AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
        AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.GetPublicKey, 100000)).
        Build();

var firstSig = rpcClient.SendTransaction(tx);
```

### Create, Initialize and Mint

```c#
var wallet = new Wallet.Wallet(MnemonicWords);

var blockHash = rpcClient.GetRecentBlockHash();
var minBalanceForExemptionAcc = rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;

var minBalanceForExemptionMint =rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;

var mintAccount = wallet.GetAccount(21);
var ownerAccount = wallet.GetAccount(10);
var initialAccount = wallet.GetAccount(22);

var tx = new TransactionBuilder().
    SetRecentBlockHash(blockHash.Result.Value.Blockhash).
    SetFeePayer(ownerAccount).
    AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        mintAccount,
        minBalanceForExemptionMint,
        TokenProgram.MintAccountDataSize,
        TokenProgram.ProgramIdKey)).
    AddInstruction(TokenProgram.InitializeMint(
        mintAccount.PublicKey,
        2,
        ownerAccount.PublicKey,
        ownerAccount.PublicKey)).
    AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        initialAccount,
        minBalanceForExemptionAcc,
        SystemProgram.AccountDataSize,
        TokenProgram.ProgramIdKey)).
    AddInstruction(TokenProgram.InitializeAccount(
        initialAccount.PublicKey,
        mintAccount.PublicKey,
        ownerAccount.PublicKey)).
    AddInstruction(TokenProgram.MintTo(
        mintAccount.PublicKey,
        initialAccount.PublicKey,
        25000,
        ownerAccount)).
    AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).Build();
```

### Transfer a Token to a new Token Account

```c#
// Initialize the rpc client and a wallet
var rpcClient = ClientFactory.GetClient(Cluster.MainNet);
var wallet = new Wallet();

var blockHash = rpcClient.GetRecentBlockHash();
var minBalanceForExemptionAcc =
    rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;

var mintAccount = wallet.GetAccount(21);
var ownerAccount = wallet.GetAccount(10);
var initialAccount = wallet.GetAccount(22);
var newAccount = wallet.GetAccount(23);

var tx = new TransactionBuilder().
    SetRecentBlockHash(blockHash.Result.Value.Blockhash).
    SetFeePayer(ownerAccount).
    AddInstruction(SystemProgram.CreateAccount(
        ownerAccount,
        newAccount,
        minBalanceForExemptionAcc,
        SystemProgram.AccountDataSize,
        TokenProgram.ProgramIdKey)).
    AddInstruction(TokenProgram.InitializeAccount(
        newAccount.PublicKey,
        mintAccount.PublicKey,
        ownerAccount.PublicKey)).
    AddInstruction(TokenProgram.Transfer(
        initialAccount.PublicKey,
        newAccount.PublicKey,
        25000,
        ownerAccount)).
    AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).Build();
```


## Contribution

We encourage everyone to contribute, submit issues, PRs, discuss. Every kind of help is welcome.

## Contributors

* **Hugo** - *Maintainer* - [murlokito](https://github.com/murlokito)
* **Tiago** - *Maintainer* - [tiago](https://github.com/tiago18c)

See also the list of [contributors](https://github.com/bmresearch/Solnet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/blob/master/LICENSE) file for details
