using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
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

    public class TransactionBuilderExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var rpcClient = ClientFactory.GetClient(Cluster.TestNet);
            var wallet = new Wallet.Wallet(MnemonicWords);

            var fromAccount = wallet.GetAccount(10);
            var toAccount = wallet.GetAccount(8);

            var blockHash = rpcClient.GetRecentBlockHash();
            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount.PublicKey, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount.PublicKey, "Hello from Sol.Net :)"))
                .Build(fromAccount);

            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            var firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Signature: {firstSig.Result}");
        }
    }

    public class CreateInitializeAndMintToExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var minBalanceForExemptionMint =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");
            
            var mintAccount = wallet.GetAccount(94206);
            Console.WriteLine($"MintAccount: {mintAccount}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            var initialAccount = wallet.GetAccount(64209);
            Console.WriteLine($"InitialAccount: {initialAccount}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    mintAccount.PublicKey,
                    minBalanceForExemptionMint,
                    TokenProgram.MintAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeMint(
                    mintAccount.PublicKey,
                    2,
                    ownerAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    initialAccount,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    initialAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(TokenProgram.MintTo(
                    mintAccount.PublicKey,
                    initialAccount.PublicKey,
                    25000,
                    ownerAccount.PublicKey))
                .AddInstruction(MemoProgram.NewMemo(initialAccount.PublicKey, "Hello from Sol.Net"))
                .Build(new List<Account> {ownerAccount, mintAccount, initialAccount});

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }

    public class TransferTokenExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc = rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var mintAccount = wallet.GetAccount(31);
            Console.WriteLine($"MintAccount: {mintAccount}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            var initialAccount = wallet.GetAccount(32);
            Console.WriteLine($"InitialAccount: {initialAccount}");
            var newAccount = wallet.GetAccount(33);
            Console.WriteLine($"NewAccount: {newAccount}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    newAccount.PublicKey,
                    minBalanceForExemptionAcc,
                    TokenProgram.TokenAccountDataSize,
                    TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                    newAccount.PublicKey,
                    mintAccount.PublicKey,
                    ownerAccount.PublicKey))
                .AddInstruction(TokenProgram.Transfer(
                    initialAccount.PublicKey,
                    newAccount.PublicKey,
                    25000,
                    ownerAccount))
                .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .Build(new List<Account> {ownerAccount, newAccount, initialAccount});

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }

    public class TransferTokenCheckedExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");

            var mintAccount = wallet.GetAccount(21);
            Console.WriteLine($"MintAccount: {mintAccount}");
            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            var initialAccount = wallet.GetAccount(26);
            Console.WriteLine($"InitialAccount: {initialAccount}");
            var newAccount = wallet.GetAccount(27);
            Console.WriteLine($"NewAccount: {newAccount}");

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                        ownerAccount.PublicKey,
                        newAccount.PublicKey,
                        minBalanceForExemptionAcc,
                        TokenProgram.TokenAccountDataSize,
                        TokenProgram.ProgramIdKey))
                .AddInstruction(TokenProgram.InitializeAccount(
                        newAccount.PublicKey,
                        mintAccount.PublicKey,
                        ownerAccount.PublicKey))
                .AddInstruction(TokenProgram.TransferChecked(
                        initialAccount.PublicKey,
                        newAccount.PublicKey,
                        25000,
                        2,
                        ownerAccount,
                        mintAccount.PublicKey))
                .AddInstruction(MemoProgram.NewMemo(
                        initialAccount,
                        "Hello from Sol.Net"))
                .Build(new List<Account> {ownerAccount, newAccount, initialAccount});

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }

    public class CreateNonceAccountExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionAcc =
                rpcClient.GetMinimumBalanceForRentExemption(NonceAccount.AccountDataSize).Result;

            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            var nonceAccount = wallet.GetAccount(1119);
            Console.WriteLine($"NonceAccount: {nonceAccount}");

            var tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount.PublicKey,
                    nonceAccount.PublicKey,
                    minBalanceForExemptionAcc,
                    NonceAccount.AccountDataSize,
                    SystemProgram.ProgramIdKey
                ))
                .AddInstruction(SystemProgram.InitializeNonceAccount(
                    nonceAccount,
                    ownerAccount))
                .Build(new List<Account> {ownerAccount, nonceAccount});


            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }

    public class TransactionBuilderTransferWithDurableNonceExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var ownerAccount = wallet.GetAccount(10);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            var nonceAccount = wallet.GetAccount(1119);
            Console.WriteLine($"NonceAccount: {nonceAccount}");
            var toAccount = wallet.GetAccount(1);
            Console.WriteLine($"ToAccount: {toAccount}");

            // Get the Nonce Account to get the Nonce to use for the transaction
            RequestResult<ResponseValue<AccountInfo>> nonceAccountInfo = rpcClient.GetAccountInfo(nonceAccount.PublicKey);
            byte[] accountDataBytes = Convert.FromBase64String(nonceAccountInfo.Result.Value.Data[0]);
            NonceAccount nonceAccountData = NonceAccount.Deserialize(accountDataBytes);
            Console.WriteLine($"NonceAccount Authority: {nonceAccountData.Authorized.Key}");
            Console.WriteLine($"NonceAccount Nonce: {nonceAccountData.Nonce.Key}");

            // Initialize the nonce information to be used with the transaction
            NonceInformation nonceInfo = new NonceInformation()
            {
                Nonce = nonceAccountData.Nonce,
                Instruction = SystemProgram.AdvanceNonceAccount(
                    nonceAccount.PublicKey,
                    ownerAccount
                )
            };

            var tx = new TransactionBuilder()
                .SetFeePayer(ownerAccount)
                .SetNonceInformation(nonceInfo)
                .AddInstruction(
                    SystemProgram.Transfer(
                        ownerAccount,
                        toAccount,
                        1_000_000_000)
                )
                .Build(ownerAccount);

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");
            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            
            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }
}