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
        /// Batch of requests and their handlers
        /// </summary>
        private List<RpcBatchReqRespItem> _reqs;

        /// <summary>
        /// Message Id generator.
        /// </summary>
        private readonly IdGenerator _idGenerator = new IdGenerator();

        /// <summary>
        /// JSON serializer options
        /// </summary>
        private JsonSerializerOptions _jsonOptions;

        /// <summary>
        /// How many requests are in this batch
        /// </summary>
        public int Count => _reqs.Count;

        /// <summary>
        /// Constructs a new SolanaRpcBatchComposer instance
        /// </summary>
        public SolanaRpcBatchComposer()
        {
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
        /// </summary>
        /// <param name="client"></param>
        public JsonRpcBatchResponse Execute(IRpcClient client)
        {
            var reqs = this.CreateJsonRequests();
            var response = client.SendBatchRequestAsync(reqs).Result;
            return ProcessBatchResponse(response.Result);
        }

        /// <summary>
        /// Handles the conversion of the generic JSON deserialized response objects to the native types.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public JsonRpcBatchResponse ProcessBatchResponse(JsonRpcBatchResponse response)
        {

            // sanity check response matches request
            if (_reqs.Count != response.Count) throw new ApplicationException($"Batch req/resp size mismatch {_reqs.Count}/{response.Count}");

            // transfer expected type info to individual
            // batch response items
            for (int ix = 0; ix < _reqs.Count; ix++)
            {
                var req = _reqs[ix];
                var resp = response[ix];

                // set the runtime type
                resp.ResultType = req.ResultType;

                // translate generic JSON deserialized content into POCO runtime types
                if (req.ResultType != null)
                {
                    resp.Result = MapJsonTypeToNativeType(resp.Result, req.ResultType);
                }

                // invoke callbacks
                if (req.Callback != null)
                {
                    req.Callback.Invoke(resp, null);
                }

            }

            // pass back the JSON batch innards
            return response;
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

        internal void AddRequest<T>(string method, IList<object> parameters, Action<T, Exception> callback)
        {
            var wrapped = WrapCallback<T>(callback);
            var handler = RpcBatchReqRespItem.Create<T>(_idGenerator.GetNextId(), method, parameters, wrapped);
            _reqs.Add(handler);
        }

        internal Task<T> AddRequest<T>(string method, IList<object> parameters)
        {
            var taskSource = new TaskCompletionSource<T>();
            var callback = WrapTaskSource<T>(taskSource);
            var handler = RpcBatchReqRespItem.Create<T>(_idGenerator.GetNextId(), method, parameters, callback);
            _reqs.Add(handler);
            return taskSource.Task;
        }

        private static Action<JsonRpcBatchResponseItem, Exception> WrapCallback<T>(Action<T, Exception> callback)
        {
            if (callback == null) return null;

            // wrap into common typed callback
            Action<JsonRpcBatchResponseItem, Exception> wrapper = (item, ex) =>
            {
                T obj = item.ResultAs<T>();
                callback.Invoke(obj, ex);
            };
            return wrapper;
        }

        private static Action<JsonRpcBatchResponseItem, Exception> WrapTaskSource<T>(TaskCompletionSource<T> taskSource)
        {
            if (taskSource == null) return null;

            // TODO - what about exceptions??

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

