using Solnet.Extensions;
using Solnet.Extensions.TokenMint;
using Solnet.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Examples
{
    public class TokenWalletExample : IExample
    {

        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {

            Wallet.Wallet wallet = new Wallet.Wallet(MnemonicWords);
            Wallet.Account ownerAccount = wallet.GetAccount(10);

            // add TokenDef for a TestNet minted token created by Solnet examples
            var tokens = new TokenMintResolver();
            tokens.Add(new TokenDef("AHRNasvVB8UDkU9knqPcn4aVfRbnbVC9HJgSTBwbx8re", "Solnet Test Token", "STT", 2));

            // load snapshot of wallet and sub-accounts
            TokenWallet tokenWallet = TokenWallet.Load(RpcClient, tokens, ownerAccount);
            var balances = tokenWallet.Balances();
            var maxsym = balances.Max(x => x.Symbol.Length);
            var maxname = balances.Max(x => x.TokenName.Length);

            // show individual token accounts
            Console.WriteLine("Individual Accounts...");
            foreach (var account in tokenWallet.TokenAccounts())
            {
                Console.WriteLine($"{account.Symbol.PadRight(maxsym)} {account.QuantityDecimal,14} {account.TokenName.PadRight(maxname)} {account.PublicKey} {(account.IsAssociatedTokenAccount ? "[ATA]" : "")}");
            }
            Console.WriteLine();

            // show filtered accounts
            Console.WriteLine("Filtered Accounts...");
            var sublist = tokenWallet.TokenAccounts().WithSymbol("STT").WithMint("AHRNasvVB8UDkU9knqPcn4aVfRbnbVC9HJgSTBwbx8re");
            foreach (var account in sublist)
            {
                Console.WriteLine($"{account.Symbol.PadRight(maxsym)} {account.QuantityDecimal,14} {account.TokenName.PadRight(maxname)} {account.PublicKey} {(account.IsAssociatedTokenAccount ? "[ATA]" : "")}");
            }
            Console.WriteLine();

            // show consolidated balances
            Console.WriteLine("Consolidated Balances...");
            foreach (var balance in tokenWallet.Balances())
            {
                Console.WriteLine($"{balance.Symbol.PadRight(maxsym)} {balance.QuantityDecimal,14} {balance.TokenName.PadRight(maxname)} in {balance.AccountCount} {(balance.AccountCount == 1 ? "account" : "accounts")}");
            }
            Console.WriteLine();

        }
    }
}