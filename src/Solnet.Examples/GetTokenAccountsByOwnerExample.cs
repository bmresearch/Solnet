using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Examples
{
    public class GetTokenAccountsByOwnerExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);
        private static readonly IRpcClient mRpcClient = ClientFactory.GetClient(Cluster.MainNet);
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public GetTokenAccountsByOwnerExample()
        {
        }

        public void Run()
        {
            Wallet.Wallet wallet = new(MnemonicWords);
            Account ownerAccount = wallet.GetAccount(10);
            RequestResult<ResponseValue<List<TokenAccount>>> token_accounts = rpcClient.GetTokenAccountsByOwner(ownerAccount.PublicKey, tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

            foreach (TokenAccount account in token_accounts.Result.Value)
            {
                Console.WriteLine(
                    $"Account: {account.PublicKey} - Mint: {account.Account.Data.Parsed.Info.Mint} - Balance: {account.Account.Data.Parsed.Info.TokenAmount.UiAmountString}");
            }

            var tokAccount = new PublicKey("CuieVDEDtLo7FypA9SbLM9saXFdb1dsshEkyErMqkRQq");
            var tokenAccounts = mRpcClient.GetTokenAccountsByOwner(tokAccount, tokenProgramId: "TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA");

            foreach (TokenAccount account in tokenAccounts.Result.Value)
            {
                Console.WriteLine(account.Account.Data.Parsed.Info.DelegatedAmount == null ?
                    $"Account: {account.PublicKey} - Mint: {account.Account.Data.Parsed.Info.Mint} - TokenBalance: {account.Account.Data.Parsed.Info.TokenAmount.UiAmountString}" :
                    $"Account: {account.PublicKey} - Mint: {account.Account.Data.Parsed.Info.Mint} - TokenBalance: {account.Account.Data.Parsed.Info.TokenAmount.UiAmountString}" +
                    $" - Delegate: {account.Account.Data.Parsed.Info.Delegate} - DelegatedBalance: {account.Account.Data.Parsed.Info.DelegatedAmount.UiAmountString}");
            }

            var delegateKey = new PublicKey("4Nd1mBQtrMJVYVfKf2PJy9NZUZdTAsp7D4xWLs4gDB4T");
            var delegateTokenAccounts = mRpcClient.GetTokenAccountsByDelegate(delegateKey, "StepAscQoEioFxxWGnh2sLBDFp9d8rvKz2Yp39iDpyT");

            foreach (TokenAccount account in delegateTokenAccounts.Result.Value)
            {
                Console.WriteLine(account.Account.Data.Parsed.Info.DelegatedAmount == null ?
                    $"Account: {account.PublicKey} - Mint: {account.Account.Data.Parsed.Info.Mint} - TokenBalance: {account.Account.Data.Parsed.Info.TokenAmount.UiAmountString}" :
                    $"Account: {account.PublicKey} - Mint: {account.Account.Data.Parsed.Info.Mint} - TokenBalance: {account.Account.Data.Parsed.Info.TokenAmount.UiAmountString}" +
                    $" - Delegate: {account.Account.Data.Parsed.Info.Delegate} - DelegatedBalance: {account.Account.Data.Parsed.Info.DelegatedAmount.UiAmountString}");
            }
        }
    }
}