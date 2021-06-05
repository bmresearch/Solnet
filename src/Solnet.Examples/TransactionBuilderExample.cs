using System;
using System.Collections.Generic;
using NBitcoin.DataEncoders;
using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;

namespace Solnet.Examples
{
    public class TransactionBuilderExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "bamboo frown napkin amount detect must wine reject sketch enrich drop wing century remind clutch grain kiss omit uncover yellow flame sketch capital child";

        public static string PrettyPrintTransactionSimulationLogs(string[] logMessages)
        {
            var logString = "";
            foreach (var log in logMessages)
            {
                logString += $"\t\t{log}\n";
            }

            return logString;
        }

        static void TransactionAndMemoExample(string[] args)
        {
            var rpcClient = ClientFactory.GetClient(Cluster.TestNet);
            var wallet = new Wallet.Wallet(MnemonicWords);

            var fromAccount = wallet.GetAccount(10);
            var toAccount = wallet.GetAccount(8);

            var blockHash = rpcClient.GetRecentBlockHash();
            //Console.WriteLine($"BlockHash >> {blockHash.Blockhash}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .AddInstruction(SystemProgram.Transfer(fromAccount.GetPublicKey, toAccount.GetPublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).Build(fromAccount);

            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            var firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Signature: {firstSig.Result}");
        }

        static void CreateInitializeAndMintToExample(string[] args)
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var minBalanceForExemptionMint =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");

            var mintAccount = wallet.GetAccount(21);
            Console.WriteLine($"MintAccount: {mintAccount.GetPublicKey}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount.GetPublicKey}");
            var initialAccount = wallet.GetAccount(22);
            Console.WriteLine($"InitialAccount: {initialAccount.GetPublicKey}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).AddInstruction(
                SystemProgram.CreateAccount(
                    ownerAccount.GetPublicKey,
                    mintAccount.GetPublicKey,
                    (long)minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramId)).AddInstruction(
                TokenProgram.InitializeMint(
                    mintAccount.GetPublicKey,
                    2,
                    ownerAccount.GetPublicKey,
                    ownerAccount.GetPublicKey)).AddInstruction(
                SystemProgram.CreateAccount(
                    ownerAccount.GetPublicKey,
                    initialAccount.GetPublicKey,
                    (long)minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramId)).AddInstruction(
                TokenProgram.InitializeAccount(
                    initialAccount.GetPublicKey,
                    mintAccount.GetPublicKey,
                    ownerAccount.GetPublicKey)).AddInstruction(
                TokenProgram.MintTo(
                    mintAccount.GetPublicKey,
                    initialAccount.GetPublicKey,
                    25000,
                    ownerAccount.GetPublicKey)).AddInstruction(
                MemoProgram.NewMemo(
                    initialAccount,
                    "Hello from Sol.Net")).Build(new List<Account> { ownerAccount, mintAccount, initialAccount });

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }


        static void ex(string[] args)
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var ownerAccount = wallet.GetAccount(0);
            Console.WriteLine($"OwnerAccount: {ownerAccount.GetPublicKey}");
            var newAccount = wallet.GetAccount(1);
            Console.WriteLine($"NewAccount: {newAccount.GetPublicKey}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)/*.AddInstruction(
                SystemProgram.CreateAccount(
                    ownerAccount.GetPublicKey,
                    newAccount.GetPublicKey,
                    (long) minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    SystemProgram.ProgramId))*/
                .AddInstruction(SystemProgram.Transfer(newAccount.GetPublicKey, ownerAccount.GetPublicKey, 100_000_000_000))
                .Build(newAccount);

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }

        static void TransferTokenChecked(string[] args)
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var minBalanceForExemptionMint =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");

            var mintAccount = wallet.GetAccount(21);
            Console.WriteLine($"MintAccount: {mintAccount.GetPublicKey}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount.GetPublicKey}");
            var initialAccount = wallet.GetAccount(26);
            Console.WriteLine($"InitialAccount: {initialAccount.GetPublicKey}");
            var newAccount = wallet.GetAccount(27);
            Console.WriteLine($"NewAccount: {newAccount.GetPublicKey}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash).AddInstruction(
                SystemProgram.CreateAccount(
                    ownerAccount.GetPublicKey,
                    newAccount.GetPublicKey,
                    (long)minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramId)).AddInstruction(
                TokenProgram.InitializeAccount(
                    newAccount.GetPublicKey,
                    mintAccount.GetPublicKey,
                    ownerAccount.GetPublicKey)).AddInstruction(
                TokenProgram.TransferChecked(
                    initialAccount.GetPublicKey,
                    newAccount.GetPublicKey,
                    25000,
                    2,
                    ownerAccount.GetPublicKey,
                    mintAccount.GetPublicKey)).AddInstruction(
                MemoProgram.NewMemo(
                    initialAccount,
                    "Hello from Sol.Net")).Build(new List<Account> { ownerAccount, newAccount });

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }
}