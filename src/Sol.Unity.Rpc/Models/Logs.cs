// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Text.Json.Serialization;

namespace Sol.Unity.Rpc.Models
{
    /// <summary>
    /// Represents a log during transaction simulation.
    /// </summary>
    public class Log
    {
        /// <summary>
        /// The error associated with the transaction simulation.
        /// </summary>
        [JsonPropertyName("err")]
        public TransactionError Error { get; set; }

        /// <summary>
        /// The log messages the transaction instructions output during execution.
        /// <remarks>
        /// This will be null if the simulation failed before the transaction was able to execute.
        /// </remarks>
        /// </summary>
        public string[] Logs { get; set; }
    }

    /// <summary>
    /// Represents a log message when subscribing to the log output of the Streaming RPC.
    /// </summary>
    public class LogInfo : Log
    {
        /// <summary>
        /// The signature of the transaction.
        /// </summary>
        public string Signature { get; set; }
    }

    /// <summary>
    /// Represents the result of a transaction simulation.
    /// </summary>
    public class SimulationLogs
    {
        /// <summary>
        /// Account infos as requested in the simulateTransaction method.
        /// </summary>
        public AccountInfo[] Accounts { get; set; }

        /// <summary>
        /// The error associated with the transaction simulation.
        /// </summary>
        [JsonPropertyName("err")]
        public TransactionError Error { get; set; }


        /// <summary>
        /// The log messages the transaction instructions output during execution.
        /// <remarks>
        /// This will be null if the simulation failed before the transaction was able to execute.
        /// </remarks>
        /// </summary>
        public string[] Logs { get; set; }
    }

    /// <summary>
    /// Represents a complete error message.
    /// </summary>
    /// <remarks>See RpcError::RpcResponseError in solana\client\src\rpc_request.rs</remarks>
    public class ErrorData : SimulationLogs
    {
        /// <summary>
        /// Represents the number of compute units consumed by the transactions.
        /// </summary>
        public ulong UnitsConsumed { get; set; }
    }
}