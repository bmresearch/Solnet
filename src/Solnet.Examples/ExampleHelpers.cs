using Solnet.Programs;
using Solnet.Rpc;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Solnet.Examples
{

    public class Examples
    {
        private static readonly IRpcClient RpcClient = ClientFactory.GetClient(Cluster.TestNet);

        public static string PrettyPrintTransactionSimulationLogs(string[] logMessages)
        {
            return logMessages.Aggregate("", (current, log) => current + $"\t\t{log}\n");
        }

        /// <summary>
        /// Submits a transaction and logs the output from SimulateTransaction.
        /// </summary>
        /// <param name="tx">The transaction data ready to simulate or submit to the network.</param>
        public static string SubmitTxSendAndLog(byte[] tx)
        {
            Console.WriteLine($"Tx Data: {Convert.ToBase64String(tx)}");

            RequestResult<ResponseValue<SimulationLogs>> txSim = RpcClient.SimulateTransaction(tx);
            string logs = PrettyPrintTransactionSimulationLogs(txSim.Result.Value.Logs);
            Console.WriteLine($"Transaction Simulation:\n\tError: {txSim.Result.Value.Error}\n\tLogs: \n" + logs);

            RequestResult<string> txReq = RpcClient.SendTransaction(tx);
            Console.WriteLine($"Tx Signature: {txReq.Result}");

            return txReq.Result;
        }

        /// <summary>
        /// Polls the rpc client until a transaction signature has been confirmed.
        /// </summary>
        /// <param name="signature">The first transaction signature.</param>
        public static void PollConfirmedTx(string signature)
        {
            RequestResult<TransactionMetaSlotInfo> txMeta = RpcClient.GetTransaction(signature);
            while (!txMeta.WasSuccessful)
            {
                Thread.Sleep(5000);
                txMeta = RpcClient.GetTransaction(signature);
            }
        }

        /// <summary>
        /// Decodes a message from wire format and logs it's content.
        /// </summary>
        /// <param name="msgData">The encoded message.</param>
        public static Message DecodeMessageFromWire(byte[] msgData)
        {

            Console.WriteLine($"Message: {Convert.ToBase64String(msgData)}");
            Message msg = Message.Deserialize(Convert.ToBase64String(msgData));

            Console.WriteLine("\n\tDECODING TRANSACTION FROM WIRE FORMAT\t");
            Console.WriteLine(
                $"Message Header: {msg.Header.RequiredSignatures} {msg.Header.ReadOnlySignedAccounts} {msg.Header.ReadOnlyUnsignedAccounts}");
            Console.WriteLine($"Message BlockHash/Nonce: {msg.RecentBlockhash}");
            foreach (PublicKey account in msg.AccountKeys)
            {
                Console.WriteLine($"Message Account: {account}");
            }

            DecodeInstructionsFromMessageAndLog(msg);

            return msg;
        }

        /// <summary>
        /// Decodes the instructions in a message and logs them.
        /// </summary>
        /// <param name="message">The message.</param>
        public static void DecodeInstructionsFromMessageAndLog(Message message)
        {
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(message);
            string aggregate = decodedInstructions.Aggregate(
                "Message Decoded Instructions:",
                (s, instruction) =>
                {
                    s += $"\n\tProgram: {instruction.ProgramName}\n\t\t\t Instruction: {instruction.InstructionName}\n";
                    return instruction.Values.Aggregate(
                        s,
                        (current, entry) =>
                            current +
                            $"\t\t\t\t{entry.Key} - {Convert.ChangeType(entry.Value, entry.Value.GetType())}\n");
                });
            Console.WriteLine(aggregate);
        }

        /// <summary>
        /// Logs the content of a transaction annd serializes it.
        /// </summary>
        /// <param name="tx"></param>
        /// <returns></returns>
        public static byte[] LogTransactionAndSerialize(Transaction tx)
        {
            Console.WriteLine($"Tx FeePayer:  {tx.FeePayer}");
            Console.WriteLine($"Tx BlockHash/Nonce: {tx.RecentBlockHash}");
            foreach (SignaturePubKeyPair signaturePubKeyPair in tx.Signatures)
            {
                Console.WriteLine(
                    $"Tx Signer: {signaturePubKeyPair.PublicKey} \tSignature: {Encoders.Base58.EncodeData(signaturePubKeyPair.Signature)}");
            }

            foreach (TransactionInstruction txInstruction in tx.Instructions)
            {
                Console.WriteLine(
                    $"Tx ProgramKey: {Encoders.Base58.EncodeData(txInstruction.ProgramId)}\n\tInstructionData: {Convert.ToBase64String(txInstruction.Data)}");
                foreach (AccountMeta accountMeta in txInstruction.Keys)
                {
                    Console.WriteLine(
                        $"Tx \tAccountMeta: {accountMeta.PublicKey}\tWritable: {accountMeta.IsWritable}\tSigner: {accountMeta.IsSigner}");
                }
            }

            return tx.Serialize();
        }
    }
}