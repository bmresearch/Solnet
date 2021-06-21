// unset

using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using System;

namespace Solnet.Examples
{
    public class NameServiceProgramExamples
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        static void Main()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionNameAcc =
                rpcClient.GetMinimumBalanceForRentExemption(NameServiceProgram.NameAccountSize + 96).Result;
            var minBalanceForExemptionNameReverseRegistry =
                rpcClient.GetMinimumBalanceForRentExemption(96+18).Result;
            Console.WriteLine($"MinBalanceForRentExemption NameAccount >> {minBalanceForExemptionNameReverseRegistry}");
            
            var payerAccount = wallet.GetAccount(10);
            var ownerAccount = wallet.GetAccount(150);
            Console.WriteLine($"PayerAccount: {payerAccount.PublicKey.Key}");
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey.Key}");

            var hashedTwitterHandle = NameServiceProgram.ComputeHashedName("hoaktrades");
            Console.WriteLine($"HashedTwitterHandle: {hashedTwitterHandle}");
            var twitterHandleRegistry = NameServiceProgram.GetTwitterHandleRegistryKey("hoaktrades");
            Console.WriteLine($"TwitterHandleRegistryKey: {twitterHandleRegistry.Key}");
            var hashedVerifiedPubkey = NameServiceProgram.ComputeHashedName(ownerAccount.PublicKey.Key);
            Console.WriteLine($"HashedVerifiedKey: {hashedVerifiedPubkey}");
            var reverseRegistry = NameServiceProgram.GetReverseRegistryKey(ownerAccount.PublicKey.Key);
            Console.WriteLine($"ReverseRegistryKey: {reverseRegistry.Key}");

            var tx = new TransactionBuilder().
                SetRecentBlockHash(blockHash.Result.Value.Blockhash).
                SetFeePayer(payerAccount).
                AddInstruction(
                    NameServiceProgram.CreateNameRegistry(
                        twitterHandleRegistry, 
                        payerAccount,
                        ownerAccount.PublicKey,
                        minBalanceForExemptionNameReverseRegistry, 
                        NameServiceProgram.NameAccountSize + 96)
                ).
                AddInstruction(
                    NameServiceProgram.CreateNameRegistry(
                        reverseRegistry, 
                        payerAccount,
                        ownerAccount.PublicKey, 
                        minBalanceForExemptionNameReverseRegistry, 
                        96+18)
                ).
                AddInstruction(MemoProgram.NewMemo(payerAccount, "Hello from Sol.Net")).Build();

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }
}