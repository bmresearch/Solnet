using Sol.Unity.Rpc.Converters;
using Sol.Unity.Rpc.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Sol.Unity.Rpc.Messages
{
    /// <summary>
    /// Holds a rpc request response.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonRpcResponse<T> : JsonRpcBase
    {
        /// <summary>
        /// The result of a given request.
        /// </summary>
        public T Result { get; set; }
    }

    /// <summary>
    /// Error message from a given request.
    /// </summary>
    [JsonConverter(typeof(RpcErrorResponseConverter))]
    public class JsonRpcErrorResponse : JsonRpcBase
    {
        /// <summary>
        /// The detailed error desserialized.
        /// </summary>
        public ErrorContent Error { get; set; }

        /// <summary>
        /// An error message.
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Holds the contents of an error message.
    /// </summary>
    public class ErrorContent
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// The string error message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Possible extension data as a dictionary.
        /// </summary>
        public ErrorData Data { get; set; }
    }

    /// <summary>
    /// Context objects, holds the slot.
    /// </summary>
    public class ContextObj
    {
        /// <summary>
        /// The slot.
        /// </summary>
        public ulong Slot { get; set; }
    }

    /// <summary>
    /// Contains the pair Context + Value from a given request.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResponseValue<T>
    {
        /// <summary>
        /// The context object from a given request.
        /// </summary>
        public ContextObj Context { get; set; }

        /// <summary>
        /// The value object from a given request.
        /// </summary>
        public T Value { get; set; }
    }
}