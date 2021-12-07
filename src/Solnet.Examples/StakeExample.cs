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
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";
        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.Account;
            Serialization.TryCreateWithSeed(fromAccount.PublicKey, "yrdy1", StakeProgram.ProgramIdKey, out PublicKey stakeAccount);
            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.CreateAccountWithSeed(
                     fromAccount,
                     stakeAccount,
                     fromAccount,
                     "yrdy1",
                     3*minbalanceforexception,
                     200,
                     StakeProgram.ProgramIdKey))
                .Build(new List<Account> { fromAccount });
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx, skipPreflight:true);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
    }
    public class AuthorizeWithSeedExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";
        public void Run()
        {  
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            var seed = wallet.DeriveMnemonicSeed();
            var b58 = new Base58Encoder();
            string f = b58.EncodeData(seed);
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.Account;
            Account toAccount = wallet.GetAccount(1);
            rpcClient.RequestAirdrop(toAccount.PublicKey, 100_000_000);
            Serialization.TryCreateWithSeed(fromAccount.PublicKey, "dog5", StakeProgram.ProgramIdKey, out PublicKey stakeAccount);

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(StakeProgram.AuthorizeWithSeed(
                    stakeAccount,
                    fromAccount,
                    f,
                    fromAccount,
                    toAccount,
                    StakeAuthorize.Staker,
                    fromAccount))
                .Build(new List<Account> { fromAccount });
            
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx, skipPreflight: true);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
    }
    public class AuthorizeExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";
        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.Account;
            Account toAccount = wallet.GetAccount(1);
            Serialization.TryCreateWithSeed(fromAccount.PublicKey, "dog1", StakeProgram.ProgramIdKey, out PublicKey stakeAccount);

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(StakeProgram.Authorize(
                    stakeAccount,
                    fromAccount,
                    toAccount,
                    StakeAuthorize.Staker,
                    fromAccount))
                .Build(new List<Account> { fromAccount });
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx, skipPreflight:true);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
    }
    public class CreateAccountWithSeedAndInitializeStakeExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";
        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.Account;
            Serialization.TryCreateWithSeed(fromAccount.PublicKey, "dog5", StakeProgram.ProgramIdKey, out PublicKey stakeAccount);
            Authorized authorized = new()
            {
                Staker = fromAccount,
                Withdrawer = fromAccount
            };
            Lockup lockup = new()
            {
                Custodian = fromAccount.PublicKey,
                Epoch = 0,
                UnixTimestamp = 0
            };

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.CreateAccountWithSeed(
                    fromAccount.PublicKey,
                    stakeAccount,
                    fromAccount.PublicKey,
                    "dog5",
                    333 * minbalanceforexception + 42,
                    StakeProgram.StakeAccountDataSize,
                    StakeProgram.ProgramIdKey))
                .AddInstruction(StakeProgram.Initialize(
                    stakeAccount,
                    authorized,
                    lockup))
                .Build(new List<Account> { fromAccount });
                //.CompileMessage();

            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
    }
    public class CreateAccountAndInitializeStakeExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";

        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            Account fromAccount = wallet.Account;
            Account stakeAccount = wallet.GetAccount(22);

            Authorized authorized = new()
            {
                Staker = fromAccount,
                Withdrawer = fromAccount
            };
            Lockup lockup = new()
            {Custodian = fromAccount.PublicKey,
                Epoch = 0,
                UnixTimestamp = 0
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
    }
    public class MasterStakeBytesExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
           "clerk shoe noise umbrella apple gold alien swap desert rubber truck okay twenty fiscal near talent drastic present leg put balcony leader access glimpse";

        public void Run()
        {
            var wallet = new Wallet.Wallet(new Mnemonic(MnemonicWords));
            rpcClient.RequestAirdrop(wallet.Account.PublicKey, 100_000_000);
            RequestResult<ResponseValue<BlockHash>> blockHash = rpcClient.GetRecentBlockHash();
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(StakeProgram.StakeAccountDataSize).Result;
            
            Account a6 = wallet.GetAccount(6);
            Account a5 = wallet.GetAccount(5);
            Account a4 = wallet.GetAccount(4);
            Account a3 = wallet.GetAccount(3);
            Account a2 = wallet.GetAccount(2);

            Authorized authorized = new()
            {
                Staker = a5,
                Withdrawer = a4
            };
            Lockup lockup = new()
            {Custodian = a3.PublicKey,
                Epoch = 0,
                UnixTimestamp = 0
            };

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(a6)
                .AddInstruction(SystemProgram.CreateAccountWithSeed(
                    a6.PublicKey,
                    a5,
                    a6,
                    "dog1",
                    3 * minbalanceforexception + 42,
                    StakeProgram.StakeAccountDataSize,
                    StakeProgram.ProgramIdKey))
                .AddInstruction(SystemProgram.Transfer(
                    a6,
                    a5,
                    5
                    ))
                .CompileMessage();
            Console.WriteLine($"Tx base64: {Convert.ToBase64String(tx)}");
            RequestResult<ResponseValue<SimulationLogs>> txSim = rpcClient.SimulateTransaction(tx);

            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);
            RequestResult<string> firstSig = rpcClient.SendTransaction(tx);
            Console.WriteLine($"First Tx Result: {firstSig.Result}");
        }
    }
}
