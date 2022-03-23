using Microsoft.Extensions.Logging;
using Sol.Unity.Rpc.Converters;
using Sol.Unity.Rpc.Messages;
using Sol.Unity.Rpc.Utilities;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sol.Unity.Rpc.Core.Http
{
    /// <summary>
    /// Base Rpc client class that abstracts the HttpClient handling.
    /// </summary>
    internal abstract class JsonRpcClient
    {
        /// <summary>
        /// The Json serializer options to be reused between calls.
        /// </summary>
        private readonly JsonSerializerOptions _serializerOptions;

        /// <summary>
        /// The HttpClient.
        /// </summary>
        private readonly HttpClient _httpClient;

        /// <summary>
        /// The logger instance.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Rate limiting strategy
        /// </summary>
        private IRateLimiter _rateLimiter;

        /// <inheritdoc cref="IRpcClient.NodeAddress"/>
        public Uri NodeAddress { get; }

        /// <summary>
        /// The internal constructor that setups the client.
        /// </summary>
        /// <param name="url">The url of the RPC server.</param>
        /// <param name="logger">The possible logger instance.</param>
        /// <param name="httpClient">The possible HttpClient instance. If null, a new instance will be created.</param>
        /// <param name="rateLimiter">An IRateLimiter instance or null for no rate limiting.</param>
        protected JsonRpcClient(string url, ILogger logger = default, HttpClient httpClient = default, IRateLimiter rateLimiter = null)
        {
            _logger = logger;
            NodeAddress = new Uri(url);
            _httpClient = httpClient ?? new HttpClient { BaseAddress = NodeAddress };
            _rateLimiter = rateLimiter;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new EncodingConverter(),
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        /// <summary>
        /// Sends a given message as a POST method and returns the deserialized message result based on the type parameter.
        /// </summary>
        /// <typeparam name="T">The type of the result to deserialize from json.</typeparam>
        /// <param name="req">The message request.</param>
        /// <returns>A task that represents the asynchronous operation that holds the request result.</returns>
        protected async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
        {
            var requestJson = JsonSerializer.Serialize(req, _serializerOptions);

            try
            {
                // pre-flight check with rate limiter if set
                _rateLimiter?.Fire(); 
                
                // logging
                _logger?.LogInformation(new EventId(req.Id, req.Method), $"Sending request: {requestJson}");

                // create byte buffer to avoid charset=utf-8 in content-type header
                // as this is rejected by some RPC nodes
                var buffer = Encoding.UTF8.GetBytes(requestJson);
                using var httpReq = new HttpRequestMessage(HttpMethod.Post, (string)null)
                {
                    Content = new ByteArrayContent(buffer)
                    {
                        Headers = {
                            { "Content-Type", "application/json"}
                        }
                    }
                };

                // execute POST
                using (var response = await _httpClient.SendAsync(httpReq).ConfigureAwait(false))
                {
                    var result = await HandleResult<T>(req, response).ConfigureAwait(false);
                    result.RawRpcRequest = requestJson;
                    return result;
                }


            }
            catch (HttpRequestException e)
            {
                var result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
                result.RawRpcRequest = requestJson;
                _logger?.LogDebug(new EventId(req.Id, req.Method), $"Caught exception: {e.Message}");
                return result;
            }
            catch (Exception e)
            {
                var result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
                result.RawRpcRequest = requestJson;
                _logger?.LogDebug(new EventId(req.Id, req.Method), $"Caught exception: {e.Message}");
                return result;
            }


        }

        /// <summary>
        /// Handles the result after sending a request.
        /// </summary>
        /// <typeparam name="T">The type of the result to deserialize from json.</typeparam>
        /// <param name="req">The original message request.</param>
        /// <param name="response">The response obtained from the request.</param>
        /// <returns>A task that represents the asynchronous operation that holds the request result.</returns>
        private async Task<RequestResult<T>> HandleResult<T>(JsonRpcRequest req, HttpResponseMessage response)
        {
            RequestResult<T> result = new RequestResult<T>(response);
            try
            {
                result.RawRpcResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _logger?.LogInformation(new EventId(req.Id, req.Method), $"Result: {result.RawRpcResponse}");
                var res = JsonSerializer.Deserialize<JsonRpcResponse<T>>(result.RawRpcResponse, _serializerOptions);

                if (res.Result != null)
                {
                    result.Result = res.Result;
                    result.WasRequestSuccessfullyHandled = true;
                }
                else
                {
                    var errorRes = JsonSerializer.Deserialize<JsonRpcErrorResponse>(result.RawRpcResponse, _serializerOptions);
                    if (errorRes is { Error: { } })
                    {
                        result.Reason = errorRes.Error.Message;
                        result.ServerErrorCode = errorRes.Error.Code;
                        result.ErrorData = errorRes.Error.Data;
                    }
                    else if(errorRes is { ErrorMessage: { } })
                    {
                        result.Reason = errorRes.ErrorMessage;
                    }
                    else
                    {
                        result.Reason = "Something wrong happened.";
                    }
                }
            }
            catch (JsonException e)
            {
                _logger?.LogDebug(new EventId(req.Id, req.Method), $"Caught exception: {e.Message}");
                result.WasRequestSuccessfullyHandled = false;
                result.Reason = "Unable to parse json.";
            }

            return result;
        }

        /// <summary>
        /// Sends a batch of messages as a POST method and returns a collection of responses.
        /// </summary>
        /// <param name="reqs">The message request.</param>
        /// <returns>A task that represents the asynchronous operation that holds the request result.</returns>
        public async Task<RequestResult<JsonRpcBatchResponse>> SendBatchRequestAsync(JsonRpcBatchRequest reqs)
        {
            if (reqs == null) throw new ArgumentNullException(nameof(reqs));
            if (reqs.Count == 0) throw new ArgumentException("Empty batch");
            var id_for_log = reqs.Min(x => x.Id);
            var requestsJson = JsonSerializer.Serialize(reqs, _serializerOptions);
            try
            {
                // pre-flight check with rate limiter if set
                _rateLimiter?.Fire(); 
                
                _logger?.LogInformation(new EventId(id_for_log, $"[batch of {reqs.Count}]"), $"Sending request: {requestsJson}");

                // create byte buffer to avoid charset=utf-8 in content-type header
                // as this is rejected by some RPC nodes
                var buffer = Encoding.UTF8.GetBytes(requestsJson);
                using var httpReq = new HttpRequestMessage(HttpMethod.Post, (string)null)
                {
                    Content = new ByteArrayContent(buffer)
                    {
                        Headers = {
                            { "Content-Type", "application/json"}
                        }
                    }
                };

                // execute POST
                using (var response = await _httpClient.SendAsync(httpReq).ConfigureAwait(false))
                {
                    var result = await HandleBatchResult(reqs, response).ConfigureAwait(false);
                    result.RawRpcRequest = requestsJson;
                    return result;
                }

            }
            catch (HttpRequestException e)
            {
                var result = new RequestResult<JsonRpcBatchResponse>(System.Net.HttpStatusCode.BadRequest, e.Message);
                result.RawRpcRequest = requestsJson;
                _logger?.LogDebug(new EventId(id_for_log, $"[batch of {reqs.Count}]"), $"Caught exception: {e.Message}");
                return result;
            }
            catch (Exception e)
            {
                var result = new RequestResult<JsonRpcBatchResponse>(System.Net.HttpStatusCode.BadRequest, e.Message);
                result.RawRpcRequest = requestsJson;
                _logger?.LogDebug(new EventId(id_for_log, $"[batch of {reqs.Count}]"), $"Caught exception: {e.Message}");
                return result;
            }

        }

        /// <summary>
        /// Handles the result after sending a batch of requests.
        /// Outcome could be a collection of failures due to a single API issue or a mixed bag of 
        /// success and failure depending on the individual request outcomes.
        /// </summary>
        /// <param name="reqs">The original batch of request messages.</param>
        /// <param name="response">The batch of responses obtained from the HTTP request.</param>
        /// <returns>A task that represents the asynchronous operation that holds the request result.</returns>
        private async Task<RequestResult<JsonRpcBatchResponse>> HandleBatchResult(JsonRpcBatchRequest reqs, HttpResponseMessage response)
        {
            var id_for_log = reqs.Min(x => x.Id);
            RequestResult<JsonRpcBatchResponse> result = new RequestResult<JsonRpcBatchResponse>(response);
            try
            {
                result.RawRpcResponse = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                _logger?.LogInformation(new EventId(id_for_log, $"[batch of {reqs.Count}]"), $"Result: {result.RawRpcResponse}");
                var res = JsonSerializer.Deserialize<JsonRpcBatchResponse>(result.RawRpcResponse, _serializerOptions);

                if (res != null)
                {
                    result.Result = res;
                    result.WasRequestSuccessfullyHandled = true;
                }
                else
                {
                    var errorRes = JsonSerializer.Deserialize<JsonRpcErrorResponse>(result.RawRpcResponse, _serializerOptions);
                    if (errorRes is { Error: { } })
                    {
                        result.Reason = errorRes.Error.Message;
                        result.ServerErrorCode = errorRes.Error.Code;
                        result.ErrorData = errorRes.Error.Data;
                    }
                    else if (errorRes is { ErrorMessage: { } })
                    {
                        result.Reason = errorRes.ErrorMessage;
                    }
                    else
                    {
                        result.Reason = "Something wrong happened.";
                    }
                }
            }
            catch (JsonException e)
            {
                _logger?.LogDebug(new EventId(id_for_log, $"[batch of {reqs.Count}]"), $"Caught exception: {e.Message}");
                result.WasRequestSuccessfullyHandled = false;
                result.Reason = "Unable to parse json.";
            }

            return result;
        }

    }

}