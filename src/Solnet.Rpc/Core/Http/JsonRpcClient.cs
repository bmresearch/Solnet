using Solnet.Rpc.Messages;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Solnet.Rpc.Core.Http
{
    /// <summary>
    /// Base Rpc client class that abstracts the HttpClient handling.
    /// </summary>
    public abstract class JsonRpcClient
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
            HttpResponseMessage response;
            RequestResult<T> result;
            var requestJson = JsonSerializer.Serialize(req, _serializerOptions);

            try
            {
                _logger?.LogInformation(new EventId(req.Id, req.Method), $"Sending request: {requestJson}");
                response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/")
                {
                    Content = new StringContent(requestJson, Encoding.UTF8, "application/json")
                });
            }
            catch (HttpRequestException e)
            {
                result = new RequestResult<T>(e.StatusCode ?? System.Net.HttpStatusCode.BadRequest, e.Message);
                _logger?.LogDebug(new EventId(req.Id, req.Method), $"Caught exception: {e.Message}");
                return result;
            }
            catch (Exception e)
            {
                result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
                _logger?.LogDebug(new EventId(req.Id, req.Method), $"Caught exception: {e.Message}");
                return result;
            }

            result = new RequestResult<T>(response);
            if (!result.WasHttpRequestSuccessful) return result;

            try
            {
                var requestRes = await response.Content.ReadAsStringAsync();

                _logger?.LogInformation(new EventId(req.Id, req.Method), $"Result: {requestRes}");

                var res = JsonSerializer.Deserialize<JsonRpcResponse<T>>(requestRes, _serializerOptions);

                if (res.Result != null)
                {
                    result.Result = res.Result;
                    result.WasRequestSuccessfullyHandled = true;
                }
                else
                {
                    var errorRes = JsonSerializer.Deserialize<JsonRpcErrorResponse>(requestRes, _serializerOptions);
                    if (errorRes is { Error: { } })
                    {
                        result.Reason = errorRes.Error.Message;
                        result.ServerErrorCode = errorRes.Error.Code;
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