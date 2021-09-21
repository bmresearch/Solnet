using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Solnet.Examples
{
    public class InstructionDecoderFromMessageExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);

        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        public void Run()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var fromAccount = wallet.GetAccount(10);
            var toAccount = wallet.GetAccount(8);

            var blockHash = rpcClient.GetRecentBlockHash();
            Console.WriteLine($"BlockHash >> {blockHash.Result.Value.Blockhash}");

            var msgBytes = new TransactionBuilder()
                .SetRecentBlockHash(blockHash.Result.Value.Blockhash)
                .SetFeePayer(fromAccount)
                .AddInstruction(SystemProgram.Transfer(fromAccount.PublicKey, toAccount.PublicKey, 10000000))
                .AddInstruction(MemoProgram.NewMemo(fromAccount.PublicKey, "Hello from Sol.Net :)"))
                .CompileMessage();

            string msgBytesBs64 = Convert.ToBase64String(msgBytes);
            Console.WriteLine($"Message Base64: {msgBytesBs64}\n");

            Console.WriteLine("\t\t\t\tDECODING MESSAGE FROM WIRE FORMAT\n");
            Message msg = Message.Deserialize(msgBytes);
            var decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            string aggregate = decodedInstructions.Aggregate(
                "Decoded Instructions:",
                (s, instruction) =>
                {
                    s += $"\n\tProgram: {instruction.ProgramName}\n\t\t\t Instruction: {instruction.InstructionName}\n";
                    return instruction.Values.Aggregate(
                        s,
                        (current, entry) =>
                            current + $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                });
            Console.WriteLine(aggregate);
        }
    }

    public class InstructionDecoderFromBlockExample : IExample
    {
        private static readonly IRpcClient rpcClient = ClientFactory.GetClient(Cluster.TestNet);
        public void Run()
        {
            var blockList = new ulong[] { 87731252, 87731314 };
            foreach (var slot in blockList)
            {
                var block = rpcClient.GetBlock(slot);
                File.WriteAllText($"./response{slot}.json", block.RawRpcResponse);
                Console.WriteLine($"BlockHash >> {block.Result.Blockhash}");

                Console.WriteLine($"\n\t\t\tDECODING INSTRUCTIONS FROM TRANSACTIONS IN BLOCK {block.Result.Blockhash}\n");

                foreach (TransactionMetaInfo txMeta in block.Result.Transactions)
                {
                    if (txMeta.Transaction.Message.Instructions.Length == 1 && "Vote111111111111111111111111111111111111111" == txMeta.Transaction.Message.AccountKeys[txMeta.Transaction.Message.Instructions[0].ProgramIdIndex]) continue;
                    if (txMeta.Transaction.Message.Instructions.Length < 2) continue;
                    Console.WriteLine($"\n\t\tDECODING INSTRUCTIONS FROM TRANSACTION {txMeta.Transaction.Signatures[0]}");

                    Console.WriteLine($"Instructions: {txMeta.Transaction.Message.Instructions.Length}");
                    Console.WriteLine($"InnerInstructions: {txMeta.Meta.InnerInstructions.Length}");
                    var decodedInstructions = InstructionDecoder.DecodeInstructions(txMeta);

                    string aggregate = decodedInstructions.Aggregate(
                        $"\tInstructions",
                        (s, instruction) =>
                        {
                            s += $"\n\tProgram: {instruction.ProgramName}\tKey: {instruction.PublicKey}\n\t\tInstruction: {instruction.InstructionName}\n";
                            s = instruction.Values.Aggregate(
                                s, (current, entry) =>
                                    current + $"\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                            if (instruction.InnerInstructions.Count > 0)
                                return instruction.InnerInstructions.Aggregate(
                                    s += $"\t\tInnerInstructions",
                                    (inner, innerInstruction) =>
                                    {
                                        inner += $"\n\t\tCPI: {innerInstruction.ProgramName}\tKey: {innerInstruction.PublicKey}\n\t\t\tInstruction: {innerInstruction.InstructionName}\n";
                                        return innerInstruction.Values.Aggregate(
                                            inner, (current, entry) =>
                                                current + $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                                    });
                            return s;
                        });
                    Console.WriteLine(aggregate);
                }

            }
        }
    }
}