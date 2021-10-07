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
- Transaction decoding from base64 and wire format and encoding back into wire format
- Message decoding from base64 and wire format and encoding back into wire format
- Instruction decompilation
- TokenWallet object to send SPL tokens and JIT provisioning of [Associated Token Accounts](https://spl.solana.com/associated-token-account)
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
- [Mango](https://github.com/bmresearch/Solnet.Mango/)

## Requirements
- net 5.0

## Dependencies
- Chaos.NaCl.Standard
- Portable.BouncyCastle

## Examples

The [Solnet.Examples](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Examples/) project contains some code examples,
essentially we're trying very hard to make it intuitive and easy to use the library.
When trying to run these examples they might lead to errors in cases where they create new accounts, in these cases, the response from the RPC
contains an and the transaction simulation logs which state that `account address is ... already in use`,
all you need to do is increment the value that is used to derive that account from the seed being used, i.e `wallet.GetAccount(value+1)`.

### Wallets

The [Solnet.Wallet](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Wallet/) project implements wallet and key generation features, these were made compatible
with both the keys generated using `solana-keygen` and the keys generated using the popular browser wallet [sollet.io](https://www.sollet.io).

#### Initializing a wallet compatible with sollet

```c#
// To initialize a wallet and have access to the same keys generated in sollet (the default)
var sollet = new Wallet("mnemonic words ...", Wordlist.English);

// Retrieve accounts by derivation path index
var account = sollet.GetAccount(10);
``` 

#### Initializing a wallet compatible with solana-keygen

```c#
// To initialize a wallet and have access to the same keys generated in solana-keygen
var wallet = new Wallet("mnemonic words ...", Wordlist.English, "passphrase", SeedMode.Bip39);

// Retrieve the account
var account = wallet.Account; // the solana-keygen mechanism does not allow account retrieval by derivation path index
```

### Generating new wallets

```c#
// Generate a new mnemonic
var newMnemonic = new Mnemonic(WordList.English, WordCount.Twelve);

var wallet = new Wallet(newMnemonic);
```


### KeyStore

The [Solnet.KeyStore](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.KeyStore/) project implements functionality to be able to securely store keys, seeds,
mnemonics, and whatever you so desire. It contains an implementation of the [Web3 Secret Storage Definition](https://github.com/ethereum/wiki/wiki/Web3-Secret-Storage-Definition)
as well as a `SolanaKeyStoreService` which can be used to read keys generated by `solana-keygen`.

#### Secret KeyStore Service

```c#
// Initialize the KeyStore
var secretKeyStoreService = new SecretKeyStoreService();

// Encrypt a private key, seed or mnemonic associated with a certain address
var jsonString = secretKeyStoreService.EncryptAndGenerateDefaultKeyStoreAsJson(password, data, address);

// Or decrypt a web3 secret storage encrypted json data
byte[] data = null;
try { 
    data = KeyStore.DecryptKeyStoreFromJson(password, jsonString);
} catch (Exception) {
    Console.WriteLine("Invalid password!");
}
```

#### Solana KeyStore Service

```c#
// Initialize the KeyStore
var secretKeyStoreService = new SolanaKeyStoreService();

// Restore a wallet from the json file generated by solana-keygen,
// with the same passphrase used when generating the keys
var wallet = secretKeyStoreService.RestoreKeystore(filePath, passphrase);
```

### RPC and Streaming RPC

The [Solnet.Rpc](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Rpc/) project contains a full-fidelity implementation of the [Solana JSON RPC](https://docs.solana.com/developing/clients/jsonrpc-api), this implementation is compatible
with both the [methods expected to be removed in v1.8](https://docs.solana.com/developing/clients/jsonrpc-api#json-rpc-api-deprecated-methods) and the methods which were added on v1.7 to replace them.

#### ClientFactory pattern

The client factory allows you to pass a `Logger` which implements the `Microsoft.Extensions.Logging.ILogger` interface.

```c#
var rpcClient = ClientFactory.GetClient(Cluster.MainNet, logger);
var streamingRpcClient = ClientFactory.GetStreamingClient(Cluster.MainNet, logger);
```

#### Using the RPC

```c#
// Get a certain account's info
var accountInfo = rpcClient.GetAccountInfo("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj");

// Or get the token accounts owned by a certain account
var tokenAccounts = rpcClient.GetTokenAccountsByOwner("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj");

// Or even filter the token accounts by the token's mint
var wrappedSolAccounts = rpcClient.GetTokenAccountsByOwner("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", "So11111111111111111111111111111111111111112");

// The following address represents Serum's address and it can be used for a number of things
var serumAddress = "9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin";

// You can query the accounts owned by a certain program, and filter based on their account data!
var programAccounts = rpcClient.GetProgramAccounts(serumAddress);
var filters = new List<MemCmp>(){ new MemCmp{ Offset = 45, Bytes = OwnerAddress } };

var filteredProgramAccounts = rpcClient.GetProgramAccounts(serumAddress, memCmpList: filters);
```

#### Using the Streaming RPC

```c#
// After having sent a transaction
var txSig = rpcClient.SendTransaction(tx);

// You can subscribe to that transaction's signature to be finalized
var subscription = streaminRpcClient.SubscribeSignature(txSig.Result, (subscriptionState, response) => {
    // do something
}, Commitment.Finalized);
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
        Build(fromAccount);

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
    AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).
    Build(new List<Account>{ ownerAccount, mintAccount, initialAccount });
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
    AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).
    Build(new List<Account>{ ownerAccount, newAccount });
```

### Transaction and Message decoding

```c#
// Given a message or transaction encoded as base64 or a byte array, you can decode it into their structures
var tx = Transaction.Deserialize(txData);
var msg = Message.Deserialize(msgData)

// This allows you to sign messages crafted by other components using Solnet
var signedTx = Transaction.Populate(msg, new List<byte[]> { account.Sign(msgData) });
```

### Programs

The [Solnet.Programs](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Programs/) project contains our implementation of several Native and SPL programs,
for brevity, they are not exemplified in depth here, but you should check out [Solnet.Examples](https://github.com/bmresearch/Solnet/tree/master/src/Solnet.Examples/) which contains numerous examples,
such as how to do multi signature operations.

### Hello Solana World

```c#
var memoInstruction = MemoProgram.NewMemo(wallet.Account, "Hello Solana World, using Solnet :)");

var recentHash = rpcClient.GetRecentBlockHash();

var tx = new TransactionBuilder().
    SetFeePayer(wallet.Account).
    AddInstruction(memoInstruction).
    SetRecentBlockHash(recentHash.Result.Value.Blockhash).
    Build(wallet.Account);
```

### Creating and sending tokens to an Associated Token Account

```c#
var recentHash = rpcClient.GetRecentBlockHash();

// By taking someone's address, derive the associated token account for their address and a corresponding mint
// NOTE: You should check if that person already has an associated token account for that mint!
PublicKey associatedTokenAccountOwner = new ("65EoWs57dkMEWbK4TJkPDM76rnbumq7r3fiZJnxggj2G");
PublicKey associatedTokenAccount =
    AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(associatedTokenAccountOwner, mintAccount);

byte[] txBytes = new TransactionBuilder().
    SetRecentBlockHash(recentHash.Result.Value.Blockhash).
    SetFeePayer(ownerAccount).
    AddInstruction(AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
        ownerAccount,
        associatedTokenAccountOwner,
        mintAccount)).
    AddInstruction(TokenProgram.Transfer(
        initialAccount,
        associatedTokenAccount,
        25000,
        ownerAccount)).// the ownerAccount was set as the mint authority
    AddInstruction(MemoProgram.NewMemo(ownerAccount, "Hello from Sol.Net")).
    Build(new List<Account> {ownerAccount});

string signature = rpcClient.SendTransaction(txBytes)
```

### Instruction decoding

```c#
// For more advanced usage, this package also has an instruction decoder
// You can deserialize a transaction's message similar to how you would using web3.js
var msg = Message.Deserialize(msgBase64);

// And you can decode all of the instructions in that message into a friendly structure
// which holds the program name, the instruction name, and parameters relevant to the instruction itself
var decodedInstructions = InstructionDecoder.DecodeInstructions(msg);
```

### Display token balances of a wallet

```c#
// load Solana token list and get RPC client
var tokens = TokenInfoResolver.Load();
var client = ClientFactory.GetClient(Cluster.MainNet);

// load snapshot of wallet and sub-accounts
TokenWallet tokenWallet = TokenWallet.Load(client, tokens, ownerAccount);
var balances = tokenWallet.Balances();

// show individual token accounts
var maxsym = balances.Max(x => x.Symbol.Length);
var maxname = balances.Max(x => x.TokenName.Length);
Console.WriteLine("Individual Accounts...");
foreach (var account in tokenWallet.TokenAccounts())
{
    Console.WriteLine($"{account.Symbol.PadRight(maxsym)} {account.BalanceDecimal,14} {account.TokenName.PadRight(maxname)} {account.PublicKey} {(account.IsAssociatedTokenAccount ? "[ATA]" : "")}");
}
Console.WriteLine();
```

### Sending an SPL token

```c#
var client = ClientFactory.GetClient(Cluster.MainNet, logger);
var tokens = New TokenInfoResolver();
var wallet = TokenWallet.Load(client, tokens, feePayer);

// find source of funds
var source = wallet.TokenAccounts.ForToken(WellKnownTokens.Serum).WithAtLeast(12.75D).FirstOrDefault();

// single-line SPL send - sends 12.75 SRM to target wallet ATA 
// if required, ATA will be created funded by feePayer
var sig = wallet.Send(source, 12.75D, target, feePayer);
```

## Contribution

We encourage everyone to contribute, submit issues, PRs, discuss. Every kind of help is welcome.

## Maintainers

* **Hugo** - [murlokito](https://github.com/murlokito)
* **Tiago** - [tiago](https://github.com/tiago18c)

See also the list of [contributors](https://github.com/bmresearch/Solnet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/blob/master/LICENSE) file for details
