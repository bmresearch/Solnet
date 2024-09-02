using Solnet.Rpc;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Programs.Abstract
{
    /// <summary>
    /// Transactional base client. Extends Base client and adds functionality related to transactions and error retrieval.
    /// </summary>
    /// <typeparam name="TEnum">The error enum type. 
    /// The enum values need to match the program error codes and be correctly mapped in BuildErrorsDictionary abstract method. </typeparam>
    public abstract class TransactionalBaseClient<TEnum> : BaseClient where TEnum : Enum
    {
        /// <summary>
        /// Mapping from error codes to error values (code, message and enum).
        /// </summary>
        protected Dictionary<uint, ProgramError<TEnum>> ProgramErrors { get; }

        /// <summary>
        /// Function that builds a mapping between error codes and error values.
        /// This is used to populate the ProgramErrors dictionary that powers the GetProgramError methods.
        /// </summary>
        /// <returns>The dictionary with the possible errors.</returns>
        protected abstract Dictionary<uint, ProgramError<TEnum>> BuildErrorsDictionary();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rpcClient"></param>
        /// <param name="streamingRpcClient"></param>
        /// <param name="programId">The program ID.</param>
        protected TransactionalBaseClient(IRpcClient rpcClient, IStreamingRpcClient streamingRpcClient, PublicKey programId) : base(rpcClient, streamingRpcClient, programId)
        {
            ProgramErrors = BuildErrorsDictionary();
        }

        /// <summary>
        /// Signs and sends a given <c>TransactionInstruction</c> using signing delegate.
        /// </summary>
        /// <param name="instruction">The transaction to be sent.</param>
        /// <param name="feePayer">The fee payer.</param>
        /// <param name="signingCallback">The callback used to sign the transaction. 
        /// This delegate is called once for each <c>PublicKey</c> account that needs write permissions according to the transaction data.</param>
        /// <param name="commitment">The commitment parameter for the RPC request.</param>
        /// <returns></returns>
        protected async Task<RequestResult<string>> SignAndSendTransaction(TransactionInstruction instruction, PublicKey feePayer, 
            Func<byte[], PublicKey, byte[]> signingCallback, Commitment commitment = Commitment.Finalized)
        {
            TransactionBuilder tb = new TransactionBuilder();
            tb.AddInstruction(instruction);

            var recentHash = await RpcClient.GetLatestBlockHashAsync();

            tb.SetRecentBlockHash(recentHash.Result.Value.Blockhash);
            tb.SetFeePayer(feePayer);

            var wireFmt = tb.CompileMessage();

            var msg = Message.Deserialize(wireFmt);

            for (int i = 0; i < msg.Header.RequiredSignatures; i++)
            {
                tb.AddSignature(signingCallback(wireFmt, msg.AccountKeys[i]));
            }

            return await RpcClient.SendTransactionAsync(tb.Serialize(), commitment: commitment);
        }

        /// <summary>
        /// Try to retrieve a custom program error from a transaction or simulation result.
        /// </summary>
        /// <param name="logs">The transaction error or simulation result.</param>
        /// <returns>The possible program error, if it was caused by this program.</returns>
        public ProgramError<TEnum> GetProgramError(SimulationLogs logs)
        {
            if (logs is { Error: { InstructionError: { Type: InstructionErrorType.Custom } } })
            {
                var id = logs.Error.InstructionError.CustomError.Value;

                if (ProgramIdKey != null && logs.Logs?.Length > 2)
                {
                    var progReturn = logs.Logs[logs.Logs.Length - 1];
                    
                    //check if error came from this program, in case its a multiple prog tx
                    if (!progReturn.StartsWith("Program " + ProgramIdKey.Key)) return null;
                }

                ProgramErrors.TryGetValue(id, out var error);
                return error;
            }
            return null;
        }

        /// <summary>
        /// Try to retrieve a custom program error from a transaction or simulation result.
        /// </summary>
        /// <param name="error">The transaction error or simulation result.</param>
        /// <returns>The possible program error, if it was caused by this program.</returns>
        public ProgramError<TEnum> GetProgramError(TransactionError error)
        {
            if (error is { InstructionError: { Type: InstructionErrorType.Custom } })
            {
                var id = error.InstructionError.CustomError.Value;

                ProgramErrors.TryGetValue(id, out var err);
                return err;
            }
            return null;
        }

    }

    /// <summary>
    /// Represents a program error and the respective message.
    /// </summary>
    /// <typeparam name="T">The underlying enum type. Enum values need to match program error codes.</typeparam>
    public class ProgramError<T> where T : Enum
    {
        /// <summary>
        /// The error kinda according to the enum.
        /// </summary>
        public T ErrorKind { get; private set; }

        /// <summary>
        /// The error message.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// The error code, according to the enum and program definition.
        /// </summary>
        public uint ErrorCode { get; private set; }

        /// <summary>
        /// Default constructor that populates all values.
        /// </summary>
        /// <param name="value">The corresponding error value.</param>
        /// <param name="message">The error message that matches the error value.</param>
        public ProgramError(T value, string message)
        {
            ErrorCode = ((IConvertible)value).ToUInt32(null);
            Message = message;
            ErrorKind = value;
        }
    }
}
