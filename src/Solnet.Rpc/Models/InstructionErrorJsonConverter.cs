using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    public class InstructionErrorJsonConverter : JsonConverter<InstructionError[]>
    {
        public override InstructionError[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var instructions = new List<InstructionError>();
            
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType != JsonTokenType.Number)
                {
                    throw new JsonException();
                }
                
                instructions.Add(
                    new InstructionError
                    {
                        ErrorCode = reader.GetInt32()
                    });
                reader.Read();

                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }
                reader.Read();
                var propertyName = reader.GetString();
                reader.Read();
                var value = reader.GetInt32();
                instructions.Add(
                    new InstructionError
                    {
                        CustomError = new KeyValuePair<string, int>(propertyName, value)
                    });
                reader.Read();
                if (reader.TokenType != JsonTokenType.EndObject)
                {
                    throw new JsonException();
                }
                reader.Read();
                if (reader.TokenType == JsonTokenType.EndArray)
                    break;
            }
            
            return instructions.ToArray();
        }

        public override void Write(Utf8JsonWriter writer, InstructionError[] value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}