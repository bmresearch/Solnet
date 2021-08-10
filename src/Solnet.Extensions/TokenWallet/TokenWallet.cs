using Solnet.Rpc;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Linq;
using System.Collections.Generic;
using Solnet.Programs;
using Solnet.Extensions.TokenInfo;

namespace Solnet.Extensions
{
    public class TokenWallet
    {
      
        /// <summary>
        /// RPC client instance
        /// </summary>
        private IRpcClient RpcClient { get; init; }

        /// <summary>
        /// Resolver for token mint
        /// </summary>
        private ITokenInfoResolver MintResolver { get; init; }

        /// <summary>
        /// PublicKey for the wallet
        /// </summary>
        public string Owner { get; init; }

        /// <summary>
        /// Cache of computed ATAs for mints
        /// </summary>
        private Dictionary<string, PublicKey> _ataCache;

        /// <summary>
        /// Native SOL balance in lamports
        /// </summary>
        private ulong _balance;

        /// <summary>
        /// List of TokenAccounts
        /// </summary>
        private List<TokenAccount> _tokenAccounts;

        /// <summary>
        /// Private constructor, get your instances via Load methods
        /// </summary>
        private TokenWallet(IRpcClient client, ITokenInfoResolver mintResolver, string publicKey)
        {
            if (client is null) throw new ArgumentNullException(nameof(client));
            if (mintResolver is null) throw new ArgumentNullException(nameof(mintResolver));
            if (publicKey is null) throw new ArgumentNullException(nameof(publicKey));
            RpcClient = client;
            MintResolver = mintResolver;
            Owner = publicKey;
            _ataCache = new Dictionary<string, PublicKey>();
        }

        /// <summary>
        /// Get a TokenWallet instance for a given public key
        /// </summary>
        /// <param name="client"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static TokenWallet Load(IRpcClient client, ITokenInfoResolver mintResolver, PublicKey publicKey)
        {
            var output = new TokenWallet(client, mintResolver, publicKey);
            output.Refresh();
            return output;
        }

        /// <summary>
        /// Refresh balances and token acconuts
        /// </summary>
        public void Refresh(Commitment commitment = Commitment.Finalized)
        {

            // get sol balance
            var balance = RpcClient.GetBalance(Owner, commitment);
            if (balance.WasSuccessful)
                _balance = balance.Result.Value;
            else
                throw new ApplicationException($"Could not load balance for {Owner}");

            // list token accounts
            var tokenAccounts = RpcClient.GetTokenAccountsByOwner(Owner.ToString(), null, TokenProgram.ProgramIdKey, commitment);
            if (tokenAccounts.WasSuccessful)
                _tokenAccounts = tokenAccounts.Result.Value;
            else
                throw new ApplicationException($"Could not load tokenAccounts for {Owner}");

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
                var balance = token.Account.Data.Parsed.Info.TokenAmount.AmountDecimal;
                if (!mintBalances.ContainsKey(mint))
                {
                    var meta = MintResolver.Resolve(mint);
                    var decimals = token.Account.Data.Parsed.Info.TokenAmount.Decimals;
                    mintBalances[mint] = new TokenWalletBalance(mint, meta.Symbol, meta.TokenName, decimals, balance, 1);
                }
                else
                    mintBalances[mint] = mintBalances[mint].AddAccount(balance, 1);
            }

            // transfer to output array
            return mintBalances.Values.OrderBy(x => x.TokenName).ToArray(); 
        }


        public TokenWalletAccount[] TokenAccounts()
        {
            var list = new List<TokenWalletAccount>();
            foreach (var account in this._tokenAccounts)
            {
                var mint = account.Account.Data.Parsed.Info.Mint;
                var ata = GetAssociatedTokenAddressForMint(mint);
                var isAta = ata.ToString() == account.PublicKey;
                var meta = MintResolver.Resolve(mint);
                var owner = account.Account.Data.Parsed.Info.Owner ?? Owner;
                var decimals = account.Account.Data.Parsed.Info.TokenAmount.Decimals;
                var balance = account.Account.Data.Parsed.Info.TokenAmount.AmountDecimal;
                list.Add(new TokenWalletAccount(mint, meta.Symbol, meta.TokenName, decimals, balance, account.PublicKey, owner, isAta));
            }
            return list.OrderBy(x => x.TokenName).ToArray();
        }



        private PublicKey GetAssociatedTokenAddressForMint(string mint) {

            if (_ataCache.ContainsKey(mint))
                return _ataCache[mint];
            else 
            {
                // derive deterministic associate token account
                // see https://spl.solana.com/associated-token-account for more info
                var address = AssociatedTokenAccountProgram.DeriveAssociatedTokenAccount(new PublicKey(Owner), new PublicKey(mint));
                _ataCache[mint] = address;
                return address;
            }

        }

    }
}
