![solnet](docs/assets/solnet-horizontal.png)
[![Build Status](https://dev.azure.com/bmresearch/solnet/_apis/build/status/bmresearch.solnet?branchName=master)](https://dev.azure.com/bmresearch/solnet/_build/latest?definitionId=1&branchName=master)

# What is Solnet?

Solnet is Solana's .NET integration library.

Solnet was developed targeting net 5.0 because we believe a modern decentralized platform deserves a modern technology framework to go along with it, although we are actively working on targeting earlier versions such as:

## Requirements
- net 5.0

## Dependencies
- NBitcoin
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

// Or initialize a mnemonic from NBitcoin before and use it
var mnemonic = new Mnemonic("mnemonic words ...");
var wallet = new Wallet(mnemonic);

``` 

### Sending a transaction

```c#
// Initialize the rpc client and a wallet
var rpcClient = new SolanaRpcClient("https://testnet.solana.com");
var wallet = new Wallet.Wallet();
// Get the source account
var fromAccount = wallet.GetAccount(0);
// Get the destination account
var toAccount = wallet.GetAccount(1);
// Get a recent block hash to include in the transaction
var blockHash = rpcClient.GetRecentBlockHash();

// Initialize a transaction builder and chain as many instructions as you want before building the message
var tx = new TransactionBuilder().
        SetRecentBlockHash(blockHash.Result.Value.Blockhash).
        AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
        AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 100000)).
        Build(fromAccount);

var firstSig = rpcClient.SendTransaction(tx);
```


## Contribution

We encourage everyone to contribute, submit issues, PRs, discuss. Every kind of help is welcome.

## Contributors

* **Hugo** - *Maintainer* - [murlokito](https://github.com/murlokito)
* **Tiago** - *Maintainer* - [tiago](https://github.com/tiago18c)

See also the list of [contributors](https://github.com/bmresearch/Solnet/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet/LICENSE) file for details
