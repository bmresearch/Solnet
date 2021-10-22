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

namespace Solnet.Examples
{
    public class StakeExample : IExample
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
            ulong minbalanceforexception = rpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            Account staker = wallet.GetAccount(8);
            Account withdrawer = wallet.GetAccount(7);
            Account custodianAccount = wallet.GetAccount(3);
            Account baseAccount = wallet.GetAccount(6);
            Account newAccount = wallet.GetAccount(9);
            Account authorizedAccount = wallet.GetAccount(5);
            Account newAuthorizedAccount = wallet.GetAccount(4);

            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            byte[] tx = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(baseAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    baseAccount.PublicKey,
                    newAccount.PublicKey,
                    minbalanceforexception,
                    TokenProgram.MintAccountDataSize,
                    StakeProgram.ProgramIdKey))
                .AddInstruction(StakeProgram.Initialize(baseAccount,
                    new Programs.Models.Stake.State.Authorized { staker = staker, withdrawer = withdrawer },
                    new Programs.Models.Stake.State.Lockup { custodian = custodianAccount.PublicKey, epoch = k.BlockHeight , unix_timestamp = DateTimeOffset.Now.ToUnixTimeSeconds()}))
                //.AddInstruction(StakeProgram.Authorize(
                //    baseAccount.PublicKey,
                //    authorizedAccount.PublicKey,
                //    newAuthorizedAccount.PublicKey,
                //    Programs.Models.Stake.State.StakeAuthorize.Staker,
                //    custodianAccount.PublicKey))
                .Build(new List<Account> { baseAccount,newAccount });
            //Console.WriteLine("Tx bytes: "+ToReadableByteArray(tx));
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
