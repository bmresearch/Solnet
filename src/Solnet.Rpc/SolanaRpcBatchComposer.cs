using Solnet.Rpc.Core;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using Solnet.Rpc.Types;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc
{
    /// <summary>
    /// This object allows a caller to compose a batch of RPC requests for batch submission
    /// </summary>
    public class SolanaRpcBatchComposer
    {

        /// <summary>
        /// The `IRpcClient` instance to use
        /// </summary>
        private IRpcClient _rpcClient;

        /// <summary>
        /// Batch of requests and their handlers
        /// </summary>
        private List<RpcBatchReqRespItem> _reqs;

        /// <summary>
        /// JSON serializer options
        /// </summary>
        private JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// How many requests are in this batch
        /// </summary>
        public int Count => _reqs.Count;

        /// <summary>
        /// Holds the auto execution mode.
        /// </summary>
        private BatchAutoExecuteMode _autoMode;

        /// <summary>
        /// Holds the batch size threshold for the auto batch execution mode.
        /// </summary>
        private int _autoBatchSize;

        /// <summary>
        /// Constructs a new SolanaRpcBatchComposer instance
        /// </summary>
        /// <param name="rpcClient">An RPC client</param>
        public SolanaRpcBatchComposer(IRpcClient rpcClient)
        {
            _rpcClient = rpcClient ?? throw new ArgumentNullException(nameof(rpcClient));
            _reqs = new List<RpcBatchReqRespItem>();
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        #region Execution


        /// <summary>
        /// Sets the auto execute mode and trigger threshold
        /// </summary>
        /// <param name="mode">The auto execute mode to use.</param>
        /// <param name="batchSizeTrigger">The number of requests that will trigger a batch execution.</param>
        public void AutoExecute(BatchAutoExecuteMode mode, int batchSizeTrigger) 
        {
            this._autoMode = mode;
            this._autoBatchSize = batchSizeTrigger;
        }

        /// <summary>
        /// Returns a batch of JSON RPC requests
        /// </summary>
        /// <returns></returns>
        public JsonRpcBatchRequest CreateJsonRequests()
        {
            var reqs = new JsonRpcBatchRequest();
            reqs.AddRange(_reqs.Select(x => x.Req));
            return reqs;
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will invoke callbacks with an exception.
        /// </summary>
        public JsonRpcBatchResponse Execute()
        {
            return ExecuteAsync(_rpcClient).Result;
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will invoke callbacks with an exception.
        /// </summary>
        /// <param name="client">The RPC client to execute this batch with</param>
        public JsonRpcBatchResponse Execute(IRpcClient client)
        {
            return ExecuteAsync(client).Result;
        }
        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will invoke callbacks with an exception.
        /// </summary>
        public async Task<JsonRpcBatchResponse> ExecuteAsync()
        { 
            return await ExecuteAsync(_rpcClient);
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will invoke callbacks with an exception.
        /// </summary>
        /// <param name="client">The RPC client to execute this batch with</param>
        public async Task<JsonRpcBatchResponse> ExecuteAsync(IRpcClient client)
        {
            var reqs = this.CreateJsonRequests();
            var response = await client.SendBatchRequestAsync(reqs);
            if (response.WasSuccessful)
                return ProcessBatchResponse(response);
            else
                return ProcessBatchFailure(response);
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will throw an Exception and bypass callbacks.
        /// </summary>
        public JsonRpcBatchResponse ExecuteWithFatalFailure()
        {
            return ExecuteWithFatalFailureAsync(_rpcClient).Result;
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will throw an Exception and bypass callbacks.
        /// </summary>
        /// <param name="client">The RPC client to execute this batch with</param>
        public JsonRpcBatchResponse ExecuteWithFatalFailure(IRpcClient client)
        {
            return ExecuteWithFatalFailureAsync(client).Result;
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will throw an Exception and bypass callbacks.
        /// </summary>
        public async Task<JsonRpcBatchResponse> ExecuteWithFatalFailureAsync()
        {
            return await ExecuteWithFatalFailureAsync(_rpcClient);
        }

        /// <summary>
        /// Execute a batch request and process the response into the expected native types.
        /// Batch failure execption will throw an Exception and bypass callbacks.
        /// </summary>
        /// <param name="client"></param>
        public async Task<JsonRpcBatchResponse> ExecuteWithFatalFailureAsync(IRpcClient client)
        {
            var reqs = this.CreateJsonRequests();
            var response = await client.SendBatchRequestAsync(reqs);
            if (response.WasSuccessful)
                return ProcessBatchResponse(response);
            else
                throw new ApplicationException($"Batch was unsuccessful: {response.Reason}");
        }

        /// <summary>
        /// Handles the conversion of the generic JSON deserialized response objects to the native types.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        internal JsonRpcBatchResponse ProcessBatchResponse(RequestResult<JsonRpcBatchResponse> response)
        {
            // sanity check response matches request
            if (response == null) throw new ArgumentNullException(nameof(response));
            if (_reqs.Count != response.Result.Count) throw new ApplicationException($"Batch req/resp size mismatch {_reqs.Count}/{response.Result.Count}");

            // transfer expected type info to individual
            // batch response items
            for (int ix = 0; ix < _reqs.Count; ix++)
            {
                var req = _reqs[ix];
                var resp = response.Result[ix];

                // set the runtime type
                resp.ResultType = req.ResultType;

                // catch any type mapping exceptions and feed into callback
                bool callbackInvoked = false;
                try
                {
                    // translate generic JSON deserialized content into POCO runtime types
                    if (req.ResultType != null)
                    {
                        resp.Result = MapJsonTypeToNativeType(resp.Result, req.ResultType);
                    }

                }
                catch (Exception ex)
                {

                    // invoke callback with the exception
                    if (req.Callback != null)
                    {
                        callbackInvoked = true;
                        req.Callback.Invoke(null, ex);
                    }

                }

                // invoke callback with safe mapped data type
                if (!callbackInvoked && req.Callback != null)
                {
                    callbackInvoked = true;
                    req.Callback.Invoke(resp, null);
                }

            }

            // reset ready for reuse
            Clear();
            
            // pass back the JSON batch innards
            return response.Result;
        }

        /// <summary>
        /// Process a failed batch response by notifying all callbacks with the exception
        /// </summary>
        /// <param name="response">The failed batch RPC response</param>
        /// <returns></returns>
        internal JsonRpcBatchResponse ProcessBatchFailure(RequestResult<JsonRpcBatchResponse> response)
        {
            // create failed batch exception
            var ex = new BatchRequestException(response);

            // transfer expected type info to individual
            // batch response items
            for (int ix = 0; ix < _reqs.Count; ix++)
            {
                // no response for each request as whole batch failed
                var req = _reqs[ix];
                req.Callback.Invoke(null, ex);
            }

            // reset ready for reuse
            Clear();

            // return the result
            return response.Result;

        }


        /// <summary>
        /// Convert a possible JsonElement type into desired response native type.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="nativeType"></param>
        /// <returns></returns>
        public object MapJsonTypeToNativeType(object input, Type nativeType)
        {
            if (input is JsonElement)
            {
                // serializes + deserializes the JSON into runtime type - suboptimal but expedient
                var elem = (JsonElement)input;
                var bufferWriter = new ArrayBufferWriter<byte>();
                using (var writer = new Utf8JsonWriter(bufferWriter))
                    elem.WriteTo(writer);
                return JsonSerializer.Deserialize(bufferWriter.WrittenSpan, nativeType, _jsonOptions);
            }
            else
            {
                return input;
            }
        }

        #endregion

        /// <summary>
        /// Clears the internal list of requests
        /// </summary>
        public void Clear()
        {
            _reqs.Clear();
        }

        /// <summary>
        /// Executes any batch using the auto execution mode (if set) or throws an execption.
        /// </summary>
        public void Flush()
        {
            switch (_autoMode)
            {
                case BatchAutoExecuteMode.ExecuteWithFatalFailure:
                    ExecuteWithFatalFailure(_rpcClient);
                    break;

                case BatchAutoExecuteMode.ExecuteWithCallbackFailures:
                    Execute(_rpcClient);
                    break;

                default:
                    throw new ApplicationException("BatchComposer AutoExecute mode not set");
            }
        }

        internal void AddRequest<T>(string method, IList<object> parameters, Action<T, Exception> callback)
        {
            var wrapped = WrapCallback<T>(callback);
            var handler = RpcBatchReqRespItem.Create<T>(_rpcClient.GetNextIdForReq(), method, parameters, wrapped);
            Add(handler);
        }

        internal Task<T> AddRequest<T>(string method, IList<object> parameters)
        {
            var taskSource = new TaskCompletionSource<T>();
            var callback = WrapTaskSource<T>(taskSource);
            var handler = RpcBatchReqRespItem.Create<T>(_rpcClient.GetNextIdForReq(), method, parameters, callback);
            Add(handler);
            return taskSource.Task;
        }

        internal void Add(RpcBatchReqRespItem task) 
        {
            // add to batch
            _reqs.Add(task);

            // does this trigger an auto execute?
            if (_autoMode != BatchAutoExecuteMode.Manual && _reqs.Count >= _autoBatchSize)
                Flush();
        }

        private static Action<JsonRpcBatchResponseItem, Exception> WrapCallback<T>(Action<T, Exception> callback)
        {
            if (callback == null) return null;

            // wrap into common typed callback
            Action<JsonRpcBatchResponseItem, Exception> wrapper = (item, ex) =>
            {
                T obj = default(T);
                if (item != null) obj = item.ResultAs<T>();
                callback.Invoke(obj, ex);
            };
            return wrapper;
        }

        private static Action<JsonRpcBatchResponseItem, Exception> WrapTaskSource<T>(TaskCompletionSource<T> taskSource)
        {
            if (taskSource == null) return null;

            // wrap into common typed callback
            Action<JsonRpcBatchResponseItem, Exception> wrapper = (item, ex) =>
            {
                T obj = item.ResultAs<T>();
                if (ex != null)
                    taskSource.SetException(ex);
                else
                    taskSource.SetResult(obj);
            };
            return wrapper;
        }

        internal static KeyValue HandleCommitment(Commitment parameter, Commitment defaultValue = Commitment.Finalized)
            => parameter != defaultValue ? KeyValue.Create("commitment", parameter) : null;

    }

    /// <summary>
    /// Encapsulates the request, the expected return type and will handle the response callback/task/delegate.
    /// </summary>
    internal class RpcBatchReqRespItem
    {
        /// <summary>
        /// Create an RpcBatchReqRespItem instance ready for execution.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="parameters"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        internal static RpcBatchReqRespItem Create<T>(int id, string method, IList<object> parameters,
                                                        Action<JsonRpcBatchResponseItem, Exception> callback)
        {
            var req = new JsonRpcRequest(id, method, parameters);
            return new RpcBatchReqRespItem(req, typeof(T), callback);
        }

        /// <summary>
        /// Construct a RpcBatchReqRespItem instance.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="resultType"></param>
        /// <param name="callback"></param>
        private RpcBatchReqRespItem(JsonRpcRequest req,
                                    Type resultType,
                                    Action<JsonRpcBatchResponseItem, Exception> callback)
        {
            this.Req = req ?? throw new ArgumentNullException(nameof(req));
            this.ResultType = resultType ?? throw new ArgumentNullException(nameof(resultType));
            this.Callback = callback;
        }

        public readonly JsonRpcRequest Req;
        public readonly Type ResultType;
        public readonly Action<JsonRpcBatchResponseItem, Exception> Callback;

    }


}

