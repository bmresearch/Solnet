![solnet](docs/assets/solnet-horizontal.png)

<p align="center">
    <a href="https://dev.azure.com/bmresearch/solnet/_build/latest?definitionId=2&branchName=refs%2Fpull%2F7%2Fmerge">
        <img src="https://img.shields.io/azure-devops/build/bmresearch/solnet/2/master?style=flat-square"
            alt="Azure DevOps Build Status (master)" ></a>
    <a href="https://img.shields.io/azure-devops/coverage/bmresearch/solnet/2/master">
        <img src="https://img.shields.io/azure-devops/coverage/bmresearch/solnet/2/master?style=flat-square"
            alt="Azure DevOps Cobertura Code Coverage (master)"></a>
    <a href="">
        <img src="https://img.shields.io/github/license/bmresearch/solnet?style=flat-square"
            alt="Code License"></a>
    <br>
    <a href="https://twitter.com/intent/follow?screen_name=blockmountainio">
        <img src="https://img.shields.io/twitter/follow/blockmountainio?style=flat-square&logo=twitter"
            alt="Follow on Twitter"></a>
    <a href="https://discord.gg/YHMbpuS3Tx">
       <img alt="Discord" src="https://img.shields.io/discord/849407317761064961?style=flat-square"
            alt="Join the discussion!"></a>
</p>

# What is Solnet?

Solnet is Solana's .NET integration library.

## Features
- Full JSON RPC API coverage
- Full Streaming JSON RPC API coverage
- Wallet and accounts (sollet and solana-keygen compatible)
- Keystore (sollet and solana-keygen compatible)
- Programs
    - System Program
    - Memo Program
    - Token Program
    - Associated Token Account Program
    - Name Service Program
    - Shared Memory Program

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
var wallet = new Wallet("mnemonic words ...", Wordlist.English, "passphrase");

// To initialize a wallet and have access to the same keys generated in sollet
var sollet = new Wallet("mnemonic words ...", Wordlist.English);
// Retrieve accounts by derivation path index
var account = sollet.GetAccount(10);

var mnemonic = new Mnemonic("mnemonic words ...");
var wallet = new Wallet(mnemonic);

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
