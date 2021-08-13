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

            var output = new TokenWallet(new TokenWalletRpcProxy(client), mintResolver, publicKey);
            var unused = await output.RefreshAsync(commitment);
            return output;
        }

        public async static Task<TokenWallet> LoadAsync(ITokenWalletRpcProxy client,
                                                        ITokenInfoResolver mintResolver,
                                                        PublicKey publicKey,
                                                        Commitment commitment = Commitment.Finalized)
        {

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
        /// Get consolidated token balances
        /// </summary>
        /// <returns></returns>
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
        /// Checks for a target ATA for the given mint and prepares one if none are found
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="mint"></param>
        /// <param name="feePayer"></param>
        /// <returns></returns>
        public PublicKey JitCreateAssociatedTokenAccount(TransactionBuilder builder, string mint, PublicKey feePayer)
        {

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





        private PublicKey GetAssociatedTokenAddressForMint(string mint)
        {

            if (_ataCache.ContainsKey(mint))
                return _ataCache[mint];
            else
            {
                // derive deterministic associate token account
                // see https://spl.solana.com/associated-token-account for more info
                var address = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(PublicKey), new PublicKey(mint));
                _ataCache[mint] = address;
                return address;
            }

        }

    }
}
