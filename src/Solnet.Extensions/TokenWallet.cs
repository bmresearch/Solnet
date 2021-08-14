using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Linq;
using System.Collections.Generic;
using Solnet.Programs;
using Solnet.Extensions.TokenInfo;
using System.Threading.Tasks;
using Solnet.Extensions.Models;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Utilities;

namespace Solnet.Extensions
{
    public class TokenWallet
    {

        /// <summary>
        /// RPC client instance
        /// </summary>
        private ITokenWalletRpcProxy RpcClient { get; init; }

        /// <summary>
        /// Resolver for token mint
        /// </summary>
        private ITokenInfoResolver MintResolver { get; init; }

        /// <summary>
        /// PublicKey for the wallet
        /// </summary>
        public string PublicKey { get; init; }

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
        private TokenWallet(ITokenWalletRpcProxy client, ITokenInfoResolver mintResolver, string publicKey)
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
        /// Get a TokenWallet instance for a given public key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="mintResolver"></param>
        /// <param name="publicKey"></param>
        /// <param name="commitment"></param>
        /// <returns></returns>
        public static TokenWallet Load(IRpcClient client,
                                       ITokenInfoResolver mintResolver,
                                       string publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(new TokenWalletRpcProxy(client), mintResolver, new PublicKey(publicKey), commitment);
            return output.Result;
        }

        public static TokenWallet Load(IRpcClient client,
                                       ITokenInfoResolver mintResolver,
                                       Account owner,
                                       Commitment commitment = Commitment.Finalized)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            var output = LoadAsync(new TokenWalletRpcProxy(client), mintResolver, owner.PublicKey, commitment);
            return output.Result;
        }

        public static TokenWallet Load(IRpcClient client,
                                       ITokenInfoResolver mintResolver,
                                       PublicKey publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(new TokenWalletRpcProxy(client), mintResolver, publicKey, commitment);
            return output.Result;
        }

        public static TokenWallet Load(ITokenWalletRpcProxy client,
                                       ITokenInfoResolver mintResolver,
                                       string publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            if (publicKey == null) throw new ArgumentNullException(nameof(publicKey));
            var output = LoadAsync(client, mintResolver, new PublicKey(publicKey), commitment);
            return output.Result;
        }

        public static TokenWallet Load(ITokenWalletRpcProxy client,
                                       ITokenInfoResolver mintResolver,
                                       Account owner,
                                       Commitment commitment = Commitment.Finalized)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            var output = LoadAsync(client, mintResolver, owner.PublicKey, commitment);
            return output.Result;
        }

        public static TokenWallet Load(ITokenWalletRpcProxy client,
                                       ITokenInfoResolver mintResolver,
                                       PublicKey publicKey,
                                       Commitment commitment = Commitment.Finalized)
        {
            var output = LoadAsync(client, mintResolver, publicKey, commitment);
            return output.Result;
        }

        public async static Task<TokenWallet> LoadAsync(IRpcClient client,
                                                        ITokenInfoResolver mintResolver,
                                                        PublicKey publicKey,
                                                        Commitment commitment = Commitment.Finalized)
        {
            return await LoadAsync(new TokenWalletRpcProxy(client), mintResolver, publicKey, commitment);
        }

        public async static Task<TokenWallet> LoadAsync(ITokenWalletRpcProxy client,
                                                        ITokenInfoResolver mintResolver,
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
        /// Refresh balances and token acconuts
        /// </summary>
        public void Refresh(Commitment commitment = Commitment.Finalized)
        {
            var unused = RefreshAsync(commitment).Result;
        }

        /// <summary>
        /// Refresh balances and token acconuts
        /// </summary>
        public async Task<bool> RefreshAsync(Commitment commitment = Commitment.Finalized)
        {

            // get sol balance and token accounts
            var balance = await RpcClient.GetBalanceAsync(PublicKey, commitment);
            var tokenAccounts = await RpcClient.GetTokenAccountsByOwnerAsync(PublicKey.ToString(), null, TokenProgram.ProgramIdKey, commitment);

            // handle balance response
            if (balance.WasSuccessful)
                Lamports = balance.Result.Value;
            else
                throw new ApplicationException($"Could not load balance for {PublicKey}");

            // handle token accounts response
            if (tokenAccounts.WasSuccessful)
                _tokenAccounts = tokenAccounts.Result.Value;
            else
                throw new ApplicationException($"Could not load tokenAccounts for {PublicKey}");

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
                    var meta = MintResolver.Resolve(mint);
                    var decimals = token.Account.Data.Parsed.Info.TokenAmount.Decimals;
                    mintBalances[mint] = new TokenWalletBalance(mint, meta.Symbol, meta.TokenName, decimals, balancDecimal, balancRaw, lamportsRaw, 1);
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
        /// <returns></returns>
        public TokenWalletFilterList TokenAccounts()
        {
            var list = new List<TokenWalletAccount>();
            foreach (var account in this._tokenAccounts)
            {
                var mint = account.Account.Data.Parsed.Info.Mint;
                var ata = GetAssociatedTokenAddressForMint(mint);
                var isAta = ata.ToString() == account.PublicKey;
                var meta = MintResolver.Resolve(mint);
                var lamportsRaw = account.Account.Lamports;
                var owner = account.Account.Data.Parsed.Info.Owner ?? PublicKey;
                var decimals = account.Account.Data.Parsed.Info.TokenAmount.Decimals;
                var balanceRaw = account.Account.Data.Parsed.Info.TokenAmount.AmountUlong;
                var balanceDecimal = account.Account.Data.Parsed.Info.TokenAmount.AmountDecimal;
                list.Add(new TokenWalletAccount(mint, meta.Symbol, meta.TokenName, decimals, balanceDecimal, balanceRaw, lamportsRaw, account.PublicKey, owner, isAta));
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
        /// <returns></returns>
        public RequestResult<string> Send(TokenWalletAccount source, decimal amount, PublicKey destination, Account feePayer)
        {
            return SendAsync(source, amount, destination, feePayer).Result;
        }

        public RequestResult<string> Send(TokenWalletAccount source, decimal amount, string destination, Account feePayer)
        {
            return SendAsync(source, amount, new PublicKey(destination), feePayer).Result;
        }

        public async Task<RequestResult<string>> SendAsync(TokenWalletAccount source, decimal amount, string destination, Account feePayer)
        {
            return await SendAsync(source, amount, new PublicKey(destination), feePayer);
        }

        public async Task<RequestResult<string>> SendAsync(TokenWalletAccount source, decimal amount, PublicKey destination, Account feePayer)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (destination == null) throw new ArgumentNullException(nameof(destination));
            if (feePayer == null) throw new ArgumentNullException(nameof(feePayer));
            if (!Ed25519Extensions.IsOnCurve(destination.KeyBytes)) throw new ArgumentException($"Destination PublicKey {destination.ToString()} is invalid wallet address.");
            if (!Ed25519Extensions.IsOnCurve(feePayer.PublicKey.KeyBytes)) throw new ArgumentException($"feePayer PublicKey {feePayer.PublicKey.ToString()} is invalid wallet address.");

            // load destination wallet
            TokenWallet destWallet = await TokenWallet.LoadAsync(RpcClient, MintResolver, destination);

            // get recent block hash
            var blockHash = await RpcClient.GetRecentBlockHashAsync();

            // prepare transaction
            var builder = new TransactionBuilder();
            builder.SetRecentBlockHash(blockHash.Result.Value.Blockhash);
            builder.SetFeePayer(feePayer);

            // create or reuse target ata for token
            var targetAta = destWallet.JitCreateAssociatedTokenAccount(builder, source.TokenMint, feePayer.PublicKey);

            // build transfer instruction
            builder.AddInstruction(
                Programs.TokenProgram.Transfer(new PublicKey(source.PublicKey),
                    targetAta, source.ConvertDecimalToUlong(amount), new PublicKey(PublicKey)));

            // execute
            var tx = builder.Build(feePayer);
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
                        feePayer, new PublicKey(PublicKey), new PublicKey(mint)));

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

            if (_ataCache.ContainsKey(mint))
                return _ataCache[mint];
            else
            {
                // derive deterministic associate token account
                // see https://spl.solana.com/associated-token-account for more info
                var targetPubkey = new PublicKey(PublicKey);
                var mintPubkey = new PublicKey(mint);
                var address = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(targetPubkey, mintPubkey);
                _ataCache[mint] = address;
                return address;
            }

        }

    }
}
