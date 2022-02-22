using Solnet.Rpc.Messages;
using Solnet.Rpc.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Converters
{
    /// <summary>
    /// Converts a TransactionError from json into its model representation.
    /// </summary>
    public class RpcErrorResponseConverter : JsonConverter<JsonRpcErrorResponse>
    {
        /// <summary>
        /// Reads and converts the JSON to type <c>JsonRpcErrorResponse</c>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert"> The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override JsonRpcErrorResponse Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject) return null;

            reader.Read();

            var err = new JsonRpcErrorResponse();

            while (reader.TokenType != JsonTokenType.EndObject)
            {
                var prop = reader.GetString();

                reader.Read();

                if ("jsonrpc" == prop)
                {
                    // do nothing
                }
                else if ("id" == prop)
                {
                    err.Id = reader.GetInt32();
                }
                else if ("error" == prop)
                {
                    if(reader.TokenType == JsonTokenType.String)
                    {
                        err.ErrorMessage = reader.GetString();
                    }
                    else if(reader.TokenType == JsonTokenType.StartObject)
                    {
                        err.Error = JsonSerializer.Deserialize<ErrorContent>(ref reader, options);
                    }
                    else
                    {
                        reader.TrySkip();
                    }
                }
                else
                {
                    reader.TrySkip();
                }

                reader.Read();
            }
            return err;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="writer">n/a</param>
        /// <param name="value">n/a</param>
        /// <param name="options">n/a</param>
        public override void Write(Utf8JsonWriter writer, JsonRpcErrorResponse value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}