using Solnet.Rpc.Messages;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Http
{
    public abstract class JsonRpcClient
    {
        private readonly JsonSerializerOptions _serializerOptions;

        private readonly HttpClient _httpClient;

        protected JsonRpcClient(string url, HttpClient httpClient = default)
        {
            _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(url) };
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
            var requestJson = JsonSerializer.Serialize(req, _serializerOptions);

            //#TODO: replace with proper logging
            Console.WriteLine($"\tRequest: {requestJson}");
            HttpResponseMessage response = null;
            RequestResult<T> result = null;

            try
            {
                response = await _httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, "/") { Content = new StringContent(requestJson, Encoding.UTF8, "application/json") });
            }
            catch (HttpRequestException e)
            {
                result = new RequestResult<T>(e.StatusCode ?? System.Net.HttpStatusCode.BadRequest, e.Message);
            }
            catch (Exception e)
            {
                result = new RequestResult<T>(System.Net.HttpStatusCode.BadRequest, e.Message);
            }

            result = new RequestResult<T>(response);
            if (result.WasHttpRequestSuccessful)
            {
                try
                {
                    var requestRes = await response.Content.ReadAsStringAsync();

                    //#TODO: replace with proper logging
                    Console.WriteLine($"\tResult: {requestRes}");
                    var res = JsonSerializer.Deserialize<JsonRpcResponse<T>>(requestRes, _serializerOptions);


                    if (res.Result != null)
                    {
                        result.Result = res.Result;
                    }
                    else
                    {
                        var errorRes = JsonSerializer.Deserialize<JsonRpcErrorResponse>(requestRes, _serializerOptions);
                        if (errorRes != null && errorRes.Error != null)
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
                    result.WasRequestSuccessfullyHandled = false;
                    result.Reason = "Unable to parse json.";
                }
            }
            return result;
        }
    }
}
