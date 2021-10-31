using Solnet.Programs;
using Solnet.Programs.Utilities;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Solnet.Programs.Models.Stake.State;
using Solnet.KeyStore;
using Solnet.Wallet.Bip39;
using Solnet.Wallet.Utilities;

namespace Solnet.Examples
{
    public class CreateAccountFromSeedExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "frame cool scissors grow gentle bag panda elegant hood creek lumber draft unhappy " +
            "cheese album crunch humor almost always always hire eyebrow unusual quit";
        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            var seed = wallet.DeriveMnemonicSeed();
            var b58 = new Base58Encoder();
            string f = b58.EncodeData(seed);
            Console.WriteLine($"Seed: {f}\nAddress: {wallet.Account.PublicKey}");
            Console.WriteLine("Hello World!");
            Console.WriteLine($"Mnemonic: {wallet.Mnemonic}");
            Console.WriteLine($"PubKey: {wallet.Account.PublicKey.Key}");
            Console.WriteLine($"PrivateKey: {wallet.Account.PrivateKey.Key}");


            var balance = rpcClient.GetBalance(wallet.Account.PublicKey);
            Console.WriteLine($"Balance: {balance.Result.Value}");

            var transactionHash = rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);

            Console.WriteLine($"TxHash: {transactionHash.Result}");
            balance = rpcClient.GetBalance(wallet.Account.PublicKey);
            Console.WriteLine($"Balance 2 : {balance.Result.Value}");

            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            var k = rpcClient.GetEpochInfo().Result;
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Console.WriteLine($"minbalance = " + minbalanceforexception);
            Account fromAccount = wallet.Account;
            Console.WriteLine($"fromAccount pubkey = {fromAccount.PublicKey}");
            var sA = Rpc.Utilities.AddressExtensions.TryCreateWithSeed(fromAccount.PublicKey, f, StakeProgram.ProgramIdKey, out PublicKey stakeAccount);
            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");
            Console.WriteLine("CreatedWithSeed = " + stakeAccount);


            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.CreateAccountWithSeed(
                     fromAccount,
                     stakeAccount,
                     fromAccount,
                     f,
                     3*minbalanceforexception,
                     200,
                     StakeProgram.ProgramIdKey))
                .Build(new List<Account> { fromAccount });
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
        static public string ToReadableByteArray(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }
    }
    public class CreateAccountWithSeedAndInitializeStakeExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            var k = rpcClient.GetEpochInfo().Result;
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Console.WriteLine(minbalanceforexception);
            Account fromAccount = wallet.GetAccount(0);
            Account stakeAccount = wallet.GetAccount(2112);

            Authorized authorized = new()
            {
                staker = fromAccount,
                withdrawer = fromAccount
            };
            Lockup lockup = new()
            {
                custodian = fromAccount.PublicKey,
                epoch = k.BlockHeight,
                unix_timestamp = DateTimeOffset.MinValue.ToUnixTimeSeconds()
            };

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.CreateAccountWithSeed(
                    fromAccount.PublicKey,
                    stakeAccount,
                    fromAccount.PublicKey,
                    "test string",
                    3*minbalanceforexception+42,
                    StakeProgram.StakeAccountDataSize,
                    fromAccount))
                .AddInstruction(StakeProgram.Initialize(
                    stakeAccount.PublicKey,
                    authorized,
                    lockup))
                .Build(new List<Account> { fromAccount });
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
        static public string ToReadableByteArray(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }
    }
    public class CreateAccountAndInitializeStakeExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            Wallet.Wallet wallet = new Wallet.Wallet(mnemonicWords: MnemonicWords);

            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            var k = rpcClient.GetEpochInfo().Result;
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.GetAccount(10);
            Account stakeAccount = wallet.GetAccount(2112);

            Authorized authorized = new()
            {
                staker = fromAccount,
                withdrawer = fromAccount
            };
            Lockup lockup = new()
            {
                custodian = fromAccount.PublicKey,
                epoch = k.BlockHeight,
                unix_timestamp = DateTimeOffset.MinValue.ToUnixTimeSeconds()
            };

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    fromAccount.PublicKey,
                    stakeAccount,
                    minbalanceforexception+42,
                    StakeProgram.StakeAccountDataSize,
                    StakeProgram.ProgramIdKey))
                .AddInstruction(StakeProgram.Initialize(
                    stakeAccount.PublicKey,
                    authorized,
                    lockup))
                .Build(new List<Account> { fromAccount, stakeAccount});
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
        static public string ToReadableByteArray(byte[] bytes)
        {
            return string.Join(", ", bytes);
        }
    }
}
