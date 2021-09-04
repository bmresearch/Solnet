using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Wallet;
using System;
using System.Collections.Generic;

namespace Solnet.Examples
{
    public class CreateNameRegistryProgramExamples : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        /// <summary>
        /// The public key of the Twitter Verification Authority.
        /// </summary>
        public static readonly PublicKey TwitterVerificationAuthorityKey = new PublicKey("867BLob5b52i81SNaV9Awm5ejkZV6VGSv9SxLcwukDDJ");

        /// <summary>
        /// The public key of the Twitter Root Parent Registry.
        /// </summary>
        public static readonly PublicKey TwitterRootParentRegistryKey = new PublicKey("AFrGkxNmVLBn3mKhvfJJABvm8RJkTtRhHDoaF97pQZaA");

        /// <summary>
        /// Get the derived account address for the reverse lookup.
        /// </summary>
        /// <param name="publicKey">The public key.</param>
        /// <returns>The derived account public key.</returns>
        public static PublicKey GetReverseRegistryKey(string publicKey)
        {
            byte[] hashedName = NameServiceProgram.ComputeHashedName(publicKey);
            PublicKey nameAccountKey = NameServiceProgram.DeriveNameAccountKey(hashedName, TwitterVerificationAuthorityKey, null);
            return nameAccountKey;
        }

        /// <summary>
        /// Get the derived account address for the twitter handle registry.
        /// </summary>
        /// <param name="twitterHandle">The twitter handle.</param>
        /// <returns>The derived account public key.</returns>
        public static PublicKey GetTwitterHandleRegistryKey(string twitterHandle)
        {
            byte[] hashedName = NameServiceProgram.ComputeHashedName(twitterHandle);
            PublicKey nameAccountKey = NameServiceProgram.DeriveNameAccountKey(hashedName, null, TwitterRootParentRegistryKey);
            return nameAccountKey;
        }

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var blockHash = rpcClient.GetRecentBlockHash();
            var minBalanceForExemptionNameAcc =
                rpcClient.GetMinimumBalanceForRentExemption(NameServiceProgram.NameAccountSize + 96).Result;

            Console.WriteLine($"MinBalanceForRentExemption NameAccount >> {minBalanceForExemptionNameAcc}");
            var minBalanceForExemptionNameReverseRegistry =
                rpcClient.GetMinimumBalanceForRentExemption(96 + 18).Result;
            Console.WriteLine($"MinBalanceForRentExemption ReverseRegistry >> {minBalanceForExemptionNameReverseRegistry}");

            var payerAccount = wallet.GetAccount(10);
            var ownerAccount = wallet.GetAccount(152);
            Console.WriteLine($"PayerAccount: {payerAccount.PublicKey.Key}");
            Console.WriteLine($"OwnerAccount: {ownerAccount.PublicKey.Key}");

            var hashedTwitterHandle = NameServiceProgram.ComputeHashedName("hoaktrades");
            Console.WriteLine($"HashedTwitterHandle: {hashedTwitterHandle}");
            var twitterHandleRegistry = GetTwitterHandleRegistryKey("hoaktrades");
            Console.WriteLine($"TwitterHandleRegistryKey: {twitterHandleRegistry.Key}");
            var hashedVerifiedPubkey = NameServiceProgram.ComputeHashedName(ownerAccount.PublicKey.Key);
            Console.WriteLine($"HashedVerifiedKey: {hashedVerifiedPubkey}");
            var reverseRegistry = GetReverseRegistryKey(ownerAccount.PublicKey.Key);
            Console.WriteLine($"ReverseRegistryKey: {reverseRegistry.Key}");

            var tx = new TransactionBuilder().SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(payerAccount).AddInstruction(
                    NameServiceProgram.CreateNameRegistry(
                        twitterHandleRegistry,
                        payerAccount,
                        ownerAccount.PublicKey,
                        minBalanceForExemptionNameReverseRegistry,
                        NameServiceProgram.NameAccountSize + 96)
                ).AddInstruction(
                    NameServiceProgram.UpdateNameRegistry(
                        reverseRegistry,
                        125,
                        new byte[] { 0, 0, 1, 1 },
                        ownerAccount.PublicKey,
                        (PublicKey)"8ZhEweTBhjTVzuRyoJteCqNU7AiHdpYTfreD1y9FvoFu")
                ).AddInstruction(MemoProgram.NewMemo(payerAccount, "Hello from Sol.Net")).CompileMessage();

            Console.WriteLine($"Tx: {Convert.ToBase64String(tx)}");

            var txSim = rpcClient.SimulateTransaction(tx);
            var logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            var txReq = rpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");
        }
    }
}