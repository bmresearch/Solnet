# Package description

Solnet is composed of several libraries, each containing their own specific functionalities. This means that you don't need to import all the assemblies if you're just looking for very specific functionality.

## Solnet.Rpc

The assembly Solnet.Rpc contains all the RPC API abstractions needed to communicate with the Solana nodes, be it using HTTP or WebSockets. This assembly contains all the models needed for such calls.
This assembly only depends on the Solnet.Wallet due to the Transactions API, which needs to be able to sign transactions.

The Solnet.Rpc assembly contains the base Transaction builder that can be used to implement your own specific programs.

## Solnet.Wallet

The Wallet assembly contains the constructs needed to initialize wallets that conform to X Y Z standards. As mentioned above, this library also contains the needed tools to sign data from a given account.

## Solnet.KeyStore

## Solnet.Programs