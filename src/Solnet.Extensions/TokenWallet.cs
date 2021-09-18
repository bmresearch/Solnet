using Solnet.Extensions.Models;
using Solnet.Extensions.TokenMint;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Rpc.Utilities;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Extensions
{
    /// <summary>
    /// An object that represents the token wallet accounts that belong to a wallet address and methods to send tokens 
    /// to other wallets whilst transparantly handling the complexities of Associated Token Accounts.
    /// <para>Use Load method to get started and Send method to send tokens.</para>
    /// </summary>
    public class TokenWallet
    {

        /// <summary>
        /// RPC client instance
        /// </summary>
        private ITokenWalletRpcProxy RpcClient { get; init; }

        /// <summary>
        /// Resolver for token mint
        /// </summary>
        private ITokenMintResolver MintResolver { get; init; }

        /// <summary>
        /// PublicKey for the wallet
        /// </summary>
        public PublicKey PublicKey { get; init; }

        /// <summary>
        /// Cache of computed ATAs for mints
        /// </summary>
        private Dictionary<string, PublicKey> _ataCache;

        /// <summary>
        /// Native SOL balance in lamports
        /// </summary>
        public ulong Lamports { get; protected set; }

        /// <summary>
        /// List of TokenAccounts
        /// </summary>
        private List<TokenAccount> _tokenAccounts;

        /// <summary>
        /// Private constructor, get your instances via Load methods
        /// </summary>
        private TokenWallet(ITokenWalletRpcProxy client, ITokenMintResolver mintResolver, PublicKey publicKey)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (mintResolver is null) throw new ArgumentNullException(nameof(mintResolver));
            if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));
            RpcClient = client;
            MintResolver = mintResolver;
            PublicKey = publicKey;
            _ataCache = new Dictionary<string, PublicKey>();
        }

        #region Overloaded Load methods

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>An instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public static TokenWallet Load(IRpcClient client,
                                       ITokenMintResolver mintResolver,
                                       string publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(new TokenWalletRpcProxy(client), mintResolver, new PublicKey(publicKey), commitment);
            return output.Result;
        }

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>An instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public static TokenWallet Load(IRpcClient client,
                                       ITokenMintResolver mintResolver,
                                       PublicKey publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(new TokenWalletRpcProxy(client), mintResolver, publicKey, commitment);
            return output.Result;
        }

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>An instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public static TokenWallet Load(ITokenWalletRpcProxy client,
                                       ITokenMintResolver mintResolver,
                                       string publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
            var output = LoadAsync(client, mintResolver, new PublicKey(publicKey), commitment);
            return output.Result;
        }

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>An instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public static TokenWallet Load(ITokenWalletRpcProxy client,
                                       ITokenMintResolver mintResolver,
                                       PublicKey publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(client, mintResolver, publicKey, commitment);
            return output.Result;
        }

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>An instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public async static Task<TokenWallet> LoadAsync(IRpcClient client,
                                                        ITokenMintResolver mintResolver,
                                                        PublicKey publicKey,
                                                        Commitment commitment = Commitment.Finalized)
        {
            return await LoadAsync(new TokenWalletRpcProxy(client), mintResolver, publicKey, commitment);
        }

        /// <summary>
        /// Load a TokenWallet instance for a given public key.
        /// </summary>
        /// <param name="client">An instance of the RPC client.</param>
        /// <param name="mintResolver">An instance of a mint resolver.</param>
        /// <param name="publicKey">The account public key.</param>
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task that results in an instance of TokenWallet loaded with the token accounts of the publicKey provided.</returns>
        public async static Task<TokenWallet> LoadAsync(ITokenWalletRpcProxy client,
                                                        ITokenMintResolver mintResolver,
                                                        PublicKey publicKey,
                                                        Commitment commitment = Commitment.Finalized)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));
            if (mintResolver == null) throw new ArgumentNullException(nameof(mintResolver));
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
            if (!Ed25519Extensions.IsOnCurve(publicKey.KeyBytes)) throw new ArgumentException("PublicKey not valid - check this is native wallet address (not an ATA, PDA or aux account)");
            var output = new TokenWallet(client, mintResolver, publicKey);
            var unused = await output.RefreshAsync(commitment);
            return output;
        }

        #endregion

        /// <summary>
        /// Refresh balances and token accounts
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// </summary>
        public void Refresh(Commitment commitment = Commitment.Finalized)
        {
            var unused = RefreshAsync(commitment).Result;
        }

        /// <summary>
        /// Refresh balances and token accounts
        /// <param name="commitment">The state commitment to consider when querying the ledger state.</param>
        /// <returns>A task that results true once complete.</returns>
        /// </summary>
        public async Task<bool> RefreshAsync(Commitment commitment = Commitment.Finalized)
        {

            // get sol balance and token accounts
            var balance = await RpcClient.GetBalanceAsync(PublicKey.Key, commitment);
            var tokenAccounts = await RpcClient.GetTokenAccountsByOwnerAsync(PublicKey.Key, null, TokenProgram.ProgramIdKey, commitment);

            // handle balance response
            if (balance.WasSuccessful)
                Lamports = balance.Result.Value;
            else
                throw new TokenWalletException($"Could not load balance for {PublicKey}", balance);

            // handle token accounts response
            if (tokenAccounts.WasSuccessful)
                _tokenAccounts = tokenAccounts.Result.Value;
            else
                throw new TokenWalletException($"Could not load tokenAccounts for {PublicKey}", tokenAccounts);

            return true;

        }

        /// <summary>
        /// Get consolidated token balances across all sub-accounts for each token mint in this wallet.
        /// </summary>
        /// <returns>An array of TokenWalletBalance objects, one per token mint in this wallet.</returns>
        public TokenWalletBalance[] Balances()
        {
            var mintBalances = new Dictionary<string, TokenWalletBalance>();
            foreach (var token in this._tokenAccounts)
            {
                var mint = token.Account.Data.Parsed.Info.Mint;
                var lamportsRaw = token.Account.Lamports;
                var balancRaw = token.Account.Data.Parsed.Info.TokenAmount.AmountUlong;
                var balancDecimal = token.Account.Data.Parsed.Info.TokenAmount.AmountDecimal;
                if (!mintBalances.ContainsKey(mint))
                {
                    var tokenDef = MintResolver.Resolve(mint);
                    var decimals = token.Account.Data.Parsed.Info.TokenAmount.Decimals;

                    // have we gained knowledge about decimal places from token account
                    // where it was previously unknown?
                    if (tokenDef.DecimalPlaces == -1 && decimals >= 0)
                        tokenDef = tokenDef.CloneWithKnownDecimals(decimals);

                    // create initial wallet balance for this mint
                    mintBalances[mint] = new TokenWalletBalance(tokenDef, balancDecimal, balancRaw, lamportsRaw, 1);

                }
                else
                    mintBalances[mint] = mintBalances[mint].AddAccount(balancDecimal, balancRaw, lamportsRaw, 1);
            }

            // transfer to output array
            return mintBalances.Values.OrderBy(x => x.TokenName).ToArray();
        }

        /// <summary>
        /// Returns a <c>TokenWalletFilterList</c> of sub-accounts in this wallet address.
        /// <para>Use the filter methods <c>ForToken</c>, <c>WithSymbol</c>, <c>WithAtLeast</c>, <c>WithMint</c>, <c>AssociatedTokenAccount</c> methods 
        /// to select the sub-account you want to use.
        /// </para>
        /// </summary>
        /// <returns>Results a list of TokenWalletAccounts for filtering.</returns>
        public TokenWalletFilterList TokenAccounts()
        {
            var list = new List<TokenWalletAccount>();
            foreach (var account in this._tokenAccounts)
            {
                var mint = account.Account.Data.Parsed.Info.Mint;
                var ata = GetAssociatedTokenAddressForMint(mint);
                var isAta = ata.ToString() == account.PublicKey;
                var lamportsRaw = account.Account.Lamports;
                var owner = account.Account.Data.Parsed.Info.Owner ?? PublicKey;
                var decimals = account.Account.Data.Parsed.Info.TokenAmount.Decimals;
                var balanceRaw = account.Account.Data.Parsed.Info.TokenAmount.AmountUlong;
                var balanceDecimal = account.Account.Data.Parsed.Info.TokenAmount.AmountDecimal;

                // resolve the TokenDef
                var tokenDef = MintResolver.Resolve(mint);

                // have we gained knowledge about decimal places from token account
                // where it was previously unknown?
                if (tokenDef.DecimalPlaces == -1 && decimals >= 0)
                    tokenDef = tokenDef.CloneWithKnownDecimals(decimals);

                // add the account instance
                list.Add(new TokenWalletAccount(tokenDef, balanceDecimal, balanceRaw, lamportsRaw, account.PublicKey, owner, isAta));
            }
            return new TokenWalletFilterList(list.OrderBy(x => x.TokenName));
        }

        /// <summary>
        /// Send tokens from source to target wallet Associated Token Account for the token mint.
        /// </summary>
        /// <para>
        /// The <c>source</c> parameter is a TokenWalletAccount instance that tokens will be sent from. 
        /// They will be deposited into an Associated Token Account in the destination wallet.
        /// If an Associated Token Account does not exist, it will be created at the cost of feePayer.
        /// </para>
        /// <param name="source">Source account of tokens to be sent.</param>
        /// <param name="amount">Human readable amount of tokens to send.</param>
        /// <param name="destination">Destination wallet address.</param>
        /// <param name="feePayer">PublicKey of the fee payer address.</param>
        /// <param name="signTxCallback">Call back function used to sign the TransactionBuilder.</param>
        /// <returns>A task that results in the transaction signature submitted to the RPC node.</returns>
        public RequestResult<string> Send(TokenWalletAccount source, decimal amount,
                                          PublicKey destination, PublicKey feePayer,
                                          Func<TransactionBuilder, byte[]> signTxCallback)
        {
            return SendAsync(source, amount, destination, feePayer, signTxCallback).Result;
        }

        /// <summary>
        /// Send tokens from source to target wallet Associated Token Account for the token mint.
        /// </summary>
        /// <para>
        /// The <c>source</c> parameter is a TokenWalletAccount instance that tokens will be sent from. 
        /// They will be deposited into an Associated Token Account in the destination wallet.
        /// If an Associated Token Account does not exist, it will be created at the cost of feePayer.
        /// </para>
        /// <param name="source">Source account of tokens to be sent.</param>
        /// <param name="amount">Human readable amount of tokens to send.</param>
        /// <param name="destination">Destination wallet address.</param>
        /// <param name="feePayer">PublicKey of the fee payer address.</param>
        /// <param name="signTxCallback">Call back function used to sign the TransactionBuilder.</param>
        /// <returns>The transaction signature submitted to the RPC node.</returns>
        public RequestResult<string> Send(TokenWalletAccount source, decimal amount,
                                          string destination, PublicKey feePayer,
                                          Func<TransactionBuilder, byte[]> signTxCallback)
        {
            return SendAsync(source, amount, new PublicKey(destination), feePayer, signTxCallback).Result;
        }

        /// <summary>
        /// Send tokens from source to target wallet Associated Token Account for the token mint.
        /// </summary>
        /// <para>
        /// The <c>source</c> parameter is a TokenWalletAccount instance that tokens will be sent from. 
        /// They will be deposited into an Associated Token Account in the destination wallet.
        /// If an Associated Token Account does not exist, it will be created at the cost of feePayer.
        /// </para>
        /// <param name="source">Source account of tokens to be sent.</param>
        /// <param name="amount">Human readable amount of tokens to send.</param>
        /// <param name="destination">Destination wallet address.</param>
        /// <param name="feePayer">PublicKey of the fee payer address.</param>
        /// <param name="signTxCallback">Call back function used to sign the TransactionBuilder.</param>
        /// <returns>A task that results in the transaction signature submitted to the RPC node.</returns>
        public async Task<RequestResult<string>> SendAsync(TokenWalletAccount source, decimal amount,
                                                           string destination, PublicKey feePayer,
                                                           Func<TransactionBuilder, byte[]> signTxCallback)
        {
            return await SendAsync(source, amount, new PublicKey(destination), feePayer, signTxCallback);
        }

        /// <summary>
        /// Send tokens from source to target wallet Associated Token Account for the token mint.
        /// </summary>
        /// <para>
        /// The <c>source</c> parameter is a TokenWalletAccount instance that tokens will be sent from. 
        /// They will be deposited into an Associated Token Account in the destination wallet.
        /// If an Associated Token Account does not exist, it will be created at the cost of feePayer.
        /// </para>
        /// <param name="source">Source account of tokens to be sent.</param>
        /// <param name="amount">Human readable amount of tokens to send.</param>
        /// <param name="destination">Destination wallet address.</param>
        /// <param name="feePayer">PublicKey of the fee payer address.</param>
        /// <param name="signTxCallback">Call back function used to sign the TransactionBuilder.</param>
        /// <returns>A task that results in the transaction signature submitted to the RPC node.</returns>
        public async Task<RequestResult<string>> SendAsync(TokenWalletAccount source, decimal amount,
                                                            PublicKey destination, PublicKey feePayer,
                                                            Func<TransactionBuilder, byte[]> signTxCallback)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (feePayer == null) throw new ArgumentNullException(nameof(feePayer));
            if (signTxCallback == null) throw new ArgumentNullException(nameof(signTxCallback));

            // are destination and feePayer valid publicKeys?
            if (!Ed25519Extensions.IsOnCurve(destination.KeyBytes)) throw new ArgumentException($"Destination PublicKey {destination.Key} is invalid wallet address.");
            if (!Ed25519Extensions.IsOnCurve(feePayer.KeyBytes)) throw new ArgumentException($"feePayer PublicKey {feePayer.Key} is invalid wallet address.");

            // make sure source account originated from this wallet
            if (source.Owner != this.PublicKey) throw new ApplicationException("Source account does not belong to this wallet.");

            // load destination wallet
            TokenWallet destWallet = await TokenWallet.LoadAsync(RpcClient, MintResolver, destination);

            // get recent block hash
            var blockHash = await RpcClient.GetRecentBlockHashAsync();

            // prepare transaction
            var builder = new TransactionBuilder();
            builder.SetRecentBlockHash(blockHash.Result.Value.Blockhash);
            builder.SetFeePayer(feePayer);

            // create or reuse target ata for token
            var targetAta = destWallet.JitCreateAssociatedTokenAccount(builder, source.TokenMint, feePayer);

            // resolve the source account TokenMint and convert to raw quantity
            var tokenDef = MintResolver.Resolve(source.TokenMint);
            var qtyRaw = tokenDef.ConvertDecimalToUlong(amount);

            // build transfer instruction
            builder.AddInstruction(
                Programs.TokenProgram.Transfer(new PublicKey(source.PublicKey),
                    targetAta, qtyRaw, PublicKey));

            // request callee sign the transaction
            var tx = signTxCallback.Invoke(builder);
            if (tx == null) throw new ApplicationException($"Result from {signTxCallback} was null");

            // execute
            return await RpcClient.SendTransactionAsync(tx);

        }


        /// <summary>
        /// Checks for a target Associated Token Account for the given mint and prepares one if not found.
        /// </summary>
        /// <para>
        /// Use this method to conditionally create a target Associated Token Account in this wallet as part of your own builder.
        /// </para>
        /// <param name="builder">The TransactionBuilder create account logic will be added to if required.</param>
        /// <param name="mint">The public key of the mint for the Associated Token Account.</param>
        /// <param name="feePayer">The account that will fund the account creation if required.</param>
        /// <returns>The public key of the Associated Token Account that will be created.</returns>
        public PublicKey JitCreateAssociatedTokenAccount(TransactionBuilder builder, string mint, PublicKey feePayer)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (mint == null) throw new ArgumentNullException(nameof(mint));
            if (feePayer == null) throw new ArgumentNullException(nameof(feePayer));
            if (!Ed25519Extensions.IsOnCurve(feePayer.KeyBytes)) throw new ArgumentException($"feePayer PublicKey {feePayer.ToString()} is invalid wallet address.");

            // find ata for this mint
            var targets = TokenAccounts().WithMint(mint).WhichAreAssociatedTokenAccounts();
            if (targets.Count() == 0)
            {
                // derive ata address
                var pubkey = GetAssociatedTokenAddressForMint(mint);

                // add instruction to create it on chain
                builder.AddInstruction(
                    AssociatedTokenAccountProgram.CreateAssociatedTokenAccount(
                        feePayer, PublicKey, new PublicKey(mint)));

                // pass back for subsequent use (transfer to etc.)
                return pubkey;

            }
            else
            {

                // use the match address
                return new PublicKey(targets.First().PublicKey);

            }

        }


        /// <summary>
        /// Compute the Associated Token Account address in this wallet for a given mint.
        /// </summary>
        /// <param name="mint">The public key of the mint for the Associated Token Account.</param>
        /// <returns>The public key of the Associated Token Account.</returns>
        private PublicKey GetAssociatedTokenAddressForMint(string mint)
        {
            if (mint == null) throw new ArgumentNullException(nameof(mint));
            if (_ataCache.ContainsKey(mint))
                return _ataCache[mint];
            else
            {
                // derive deterministic associate token account
                // see https://spl.solana.com/associated-token-account for more info
                var targetPubkey = PublicKey;
                var mintPubkey = new PublicKey(mint);
                var address = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(targetPubkey, mintPubkey);
                _ataCache[mint] = address;
                return address;
            }

        }

    }
}