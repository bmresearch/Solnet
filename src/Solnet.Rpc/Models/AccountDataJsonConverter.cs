using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Implements a custom JSON converter to parse the different types of account data.
    /// </summary>
    public class AccountDataJsonConverter : JsonConverter<object>
    {
        /// <inheritdoc/>
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if(reader.TokenType == JsonTokenType.StartArray)
            {
                return JsonSerializer.Deserialize<List<string>>(ref reader, options);
            } 
            else if (reader.TokenType == JsonTokenType.StartObject)
            {
                return JsonSerializer.Deserialize<TokenAccountData>(ref reader, options);
            }
            return null;
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}