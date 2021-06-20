using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Examples
{
    public class Examples
    {
        public static string PrettyPrintTransactionSimulationLogs(string[] logMessages)
        {
            var logString = "";
                foreach (var log in logMessages)
            {
                logString += $"\t\t{log}\n";
            }

            return logString;
        }
    }

    public class TransactionBuilderExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";



        static void Example(string[] args)
        {
            var rpcClient = ClientFactory.GetClient(Cluster.TestNet);
            var wallet = new Wallet.Wallet(MnemonicWords);

            var fromAccount = wallet.GetAccount(10);
            var toAccount = wallet.GetAccount(8);

            var blockHash = rpcClient.GetRecentBlockHash();
            //Console.WriteLine($"BlockHash >> {blockHash.Blockhash}");

            var tx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(fromAccount).
                AddInstruction(SystemProgram.Transfer(fromAccount, toAccount.PublicKey, 10000000)).
                AddInstruction(MemoProgram.NewMemo(fromAccount, "Hello from Sol.Net :)")).
                Build();

            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
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

            var mintAccount = wallet.GetAccount(37);
            Console.WriteLine($"MintAccount: {mintAccount.PublicKey}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey}");
            var initialAccount = wallet.GetAccount(38);
            Console.WriteLine($"InitialAccount: {initialAccount.PublicKey}");

            var tx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(ownerAccount).
                AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
                    minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey)).
                AddInstruction(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey)).
                AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramIdKey)).
                AddInstruction(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey)).
                AddInstruction(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount)).
                AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).Build();

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }


        static void TransferTokenExample(string[] args)
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc = rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var mintAccount = wallet.GetAccount(31);
            Console.WriteLine($"MintAccount: {mintAccount.PublicKey}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey}");
            var initialAccount = wallet.GetAccount(32);
            Console.WriteLine($"InitialAccount: {initialAccount.PublicKey}");
            var newAccount = wallet.GetAccount(33);
            Console.WriteLine($"NewAccount: {newAccount.PublicKey}");

            var tx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(ownerAccount).
                AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    newAccount,
                    minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramIdKey)).
                AddInstruction(TokenProgram.InitializeAccount(
                    newAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey)).
                AddInstruction(TokenProgram.Transfer(
                    initialAccount.PublicKey,
                    newAccount.PublicKey,
                    25000,
                    ownerAccount)).
                AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net")).Build();

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
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

            var mintAccount = wallet.GetAccount(21);
            Console.WriteLine($"MintAccount: {mintAccount.PublicKey}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey}");
            var initialAccount = wallet.GetAccount(26);
            Console.WriteLine($"InitialAccount: {initialAccount.PublicKey}");
            var newAccount = wallet.GetAccount(27);
            Console.WriteLine($"NewAccount: {newAccount.PublicKey}");

            var tx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(ownerAccount).
                AddInstruction(
                SystemProgram.CreateAccount(
                    ownerAccount,
                    newAccount,
                    minBalanceForExemptionAcc,
                    SystemProgram.AccountDataSize,
                    TokenProgram.ProgramIdKey)).AddInstruction(
                TokenProgram.InitializeAccount(
                    newAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey)).AddInstruction(
                TokenProgram.TransferChecked(
                    initialAccount.PublicKey,
                    newAccount.PublicKey,
                    25000,
                    2,
                    ownerAccount,
                    mintAccount.PublicKey)).AddInstruction(
                MemoProgram.NewMemo(
                    initialAccount,
                    "Hello from Sol.Net")).Build();

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }

        static void InitializeMultiSignatureExample()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(SystemProgram.AccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var minBalanceForExemptionMint =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");

        }
    }
}