using System;
using System.Linq;
using System.Text;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Util;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class TransactionBuilderExample
    {
        public static string PrettyPrintTransactionSimulationLogs(Log logMessage)
        {
            var logString = "";
            foreach (var log in logMessage.Logs)
            {
                logString += $"\t\t{log}\n";
            }

            return logString;
        }
        
        static void TransactionAndMemoExample(string[] args)
        {
            var rpcClient = new SolanaRpcClient("https://testnet.solana.com");
            var wallet = new Wallet.Wallet("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");

            var fromAccount = wallet.GetAccount(0);
            var toAccount = wallet.GetAccount(1);

            var blockHash = rpcClient.GetRecentBlockHash();
            //Console.WriteLine($"BlockHash >> {blockHash.Blockhash}");
            
            var tx = new TransactionBuilder().
                    SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                    AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
                    AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 10000000)).
                    Build(fromAccount);
            
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs );
            var firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Signature: {firstSig.Result}");
        }

        static void TokenMintExample(string[] args)
        {
            var rpcClient = new SolanaRpcClient("https://testnet.solana.com");
            var wallet = new Wallet.Wallet("route clerk disease box emerge airport loud waste attitude film army tray forward deal onion eight catalog surface unit card window walnut wealth medal");

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemption = rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result.Value;

            var mintAccount = wallet.GetAccount(3);
            var ownerAccount = wallet.GetAccount(4);
            var initialAccount = wallet.GetAccount(5);

            var txBuilder = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.GetPublicKey,
                    mintAccount.GetPublicKey,
                    minBalanceForExemption,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramId));

        }
    }
}