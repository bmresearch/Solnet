// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ClassNeverInstantiated.Global

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
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
        public string Error { get; set; }

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
    /*
    public class InstructionError
    {
        public int ErrorCode { get; set; }

        public KeyValuePair<string, int> CustomError { get; set; }
    }
    */





    public class SimulationLogs
    {
        public object[] Accounts { get; set; }

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
}
