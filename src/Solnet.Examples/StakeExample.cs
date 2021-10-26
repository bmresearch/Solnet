using Solnet.Programs;
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

namespace Solnet.Examples
{
    public class CreateAndInitializeStakeExample : IExample
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
