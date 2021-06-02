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
    public abstract class JsonRpcClient
    {
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly HttpClient _httpClient;

        private readonly ILogger _logger;

        protected JsonRpcClient(string url, ILogger logger, HttpClient httpClient = default) : this(url, httpClient)
        {
            _logger = logger;
        }

        protected JsonRpcClient(string url, HttpClient httpClient = default)
        {
            _httpClient = httpClient ?? new HttpClient {BaseAddress = new Uri(url)};
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
                }
            };
        }

        protected async Task<RequestResult<T>> SendRequest<T>(JsonRpcRequest req)
        {
            HttpResponseMessage response;
            RequestResult<T> result;
            var requestJson = JsonSerializer.Serialize(req, _serializerOptions);

            try
            {
                _logger?.LogInformation(new EventId(req.Id, req.Method), $"Sending request: {requestJson}");
                response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/")
                    {Content = new StringContent(requestJson, Encoding.UTF8, "application/json")});
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
                    if (errorRes is {Error: { }})
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