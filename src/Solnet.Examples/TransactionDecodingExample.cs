using Solnet.Programs;
using Solnet.Programs.Models;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Examples
{
    public class TransactionDecodingExample : IExample
    {
        
        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            RequestResult<ResponseValue<BlockHash>> blockHash = RpcClient.GetRecentBlockHash();
            ulong minBalanceForExemptionAcc =
                RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.TokenAccountDataSize).Result;
            ulong minBalanceForExemptionMint =
                RpcClient.GetMinimumBalanceForRentExemption(TokenProgram.MintAccountDataSize).Result;

            Console.WriteLine($"MinBalanceForRentExemption Account >> {minBalanceForExemptionAcc}");
            Console.WriteLine($"MinBalanceForRentExemption Mint Account >> {minBalanceForExemptionMint}");

            Account ownerAccount = wallet.GetAccount(10);
            Account mintAccount = wallet.GetAccount(1002);
            Account initialAccount = wallet.GetAccount(1102);
            Console.WriteLine($"OwnerAccount: {ownerAccount}");
            Console.WriteLine($"MintAccount: {mintAccount}");
            Console.WriteLine($"InitialAccount: {initialAccount}");

            byte[] msgData = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(ownerAccount)
                .AddInstruction(SystemProgram.CreateAccount(
                    ownerAccount,
                    mintAccount,
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
                    1_000_000,
                    ownerAccount))
                .AddInstruction(MemoProgram.NewMemo(initialAccount, "Hello from Sol.Net"))
                .CompileMessage();


            Console.WriteLine($"Message: {Convert.ToBase64String(msgData)}");
            var txx = Transaction.Populate(Convert.ToBase64String(msgData),
                new List<byte[]> {ownerAccount.Sign(msgData), mintAccount.Sign(msgData), initialAccount.Sign(msgData)});

            byte[] txBytes = txx.Serialize();
            string txBytesBs64 = Convert.ToBase64String(txBytes);
            
            string help = txBytes.Aggregate("[", (current, txByte) => current + $"{txByte}, ");
            help = help.TrimEnd()[..(help.Length - 2)] + "]";
            Console.WriteLine($"TxBytes: {help}");
            Console.WriteLine($"Tx: {txBytesBs64}");

            var txSim = RpcClient.SimulateTransaction(txBytes);
            string logs = Examples.PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            Console.WriteLine("\tDECODING TRANSACTION FROM WIRE FORMAT\t");
            Transaction tx = Transaction.Deserialize(txBytes);
            Console.WriteLine($"FeePayer: {tx.FeePayer}");
            Console.WriteLine($"BlockHash/Nonce: {tx.RecentBlockHash}");
            foreach (SignaturePubKeyPair signaturePubKeyPair in tx.Signatures)
            {
                Console.WriteLine(
                    $"Signer: {signaturePubKeyPair.PublicKey} \tSignature: {Encoders.Base58.EncodeData(signaturePubKeyPair.Signature)}");
            }

            foreach (TransactionInstruction txInstruction in tx.Instructions)
            {
                Console.WriteLine(
                    $"ProgramKey: {Encoders.Base58.EncodeData(txInstruction.ProgramId)}\n\tInstructionData: {Convert.ToBase64String(txInstruction.Data)}");
                foreach (AccountMeta accountMeta in txInstruction.Keys)
                {
                    Console.WriteLine(
                        $"\tAccountMeta: {accountMeta.PublicKey}\tWritable: {accountMeta.IsWritable}\tSigner: {accountMeta.IsSigner}");
                }
            }

            var txDecBytes = tx.Serialize();
            string txDecBytesBs64 = Convert.ToBase64String(txDecBytes);
            string txDecHelp = txDecBytes.Aggregate("[", (current, txByte) => current + $"{txByte}, ");
            txDecHelp = txDecHelp.TrimEnd()[..(txDecHelp.Length - 2)] + "]";
            Console.WriteLine($"Recompiled Transaction Bytes: {txDecHelp}");
            Console.WriteLine($"Recompiled Transaction Base64: {txDecBytesBs64}");
            var txDecSim = RpcClient.SimulateTransaction(txDecBytes);
            string decLogs = Examples.PrettyPrintTransactionSimulationLogs(txDecSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txDecSim.Result.Value.Error}\n\tLogs: \n" + decLogs);
            Console.WriteLine($"{txBytesBs64 == txDecBytesBs64}");
        }
    }
}