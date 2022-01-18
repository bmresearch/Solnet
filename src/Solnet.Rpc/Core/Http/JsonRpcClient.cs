using Microsoft.Extensions.Logging;
using Solnet.Rpc.Converters;
using Solnet.Rpc.Messages;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Http
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

        /// <inheritdoc cref="IRpcClient.NodeAddress"/>
        public Uri NodeAddress { get; }

        /// <summary>
        /// The internal constructor that setups the client.
        /// </summary>
        /// <param name="url">The url of the RPC server.</param>
        /// <param name="logger">The possible logger instance.</param>
        /// <param name="httpClient">The possible HttpClient instance. If null, a new instance will be created.</param>
        protected JsonRpcClient(string url, ILogger logger = default, HttpClient httpClient = default)
        {
            _logger = logger;
            NodeAddress = new Uri(url);
            _httpClient = httpClient ?? new HttpClient { BaseAddress = NodeAddress };
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
                var result = new RequestResult<T>(e.StatusCode ?? System.Net.HttpStatusCode.BadRequest, e.Message);
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
    }
}