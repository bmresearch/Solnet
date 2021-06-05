using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    public class TransactionErrorJsonConverter : JsonConverter<TransactionError>
    {
        public override TransactionError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            var err = new TransactionError();

            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = reader.GetString();

                Enum.TryParse(enumValue, ignoreCase: false, out TransactionErrorType errorType);
                err.Type = errorType;
                reader.Read();
                return err;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Unexpected error value.");
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Unexpected error value.");
            }


            {
                var enumValue = reader.GetString();
                Enum.TryParse(enumValue, ignoreCase: false, out TransactionErrorType errorType);
                err.Type = errorType;
            }

            reader.Read();
            err.InstructionError = new InstructionError();

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException("Unexpected error value.");
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException("Unexpected error value.");
            }

            err.InstructionError.InstructionIndex = reader.GetInt32();

            reader.Read();

            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = reader.GetString();

                Enum.TryParse(enumValue, ignoreCase: false, out InstructionErrorType errorType);
                err.InstructionError.Type = errorType;
                reader.Read(); //string

                reader.Read(); //endarray
                return err;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException("Unexpected error value.");
            }

            reader.Read();


            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException("Unexpected error value.");
            }
            {
                var enumValue = reader.GetString();
                Enum.TryParse(enumValue, ignoreCase: false, out InstructionErrorType errorType);
                err.InstructionError.Type = errorType;
            }

            reader.Read();

            if (reader.TokenType == JsonTokenType.Number)
            {
                err.InstructionError.CustomError = reader.GetInt32();
                reader.Read(); //number
                reader.Read(); //endobj
                reader.Read(); //endarray

                return err;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException("Unexpected error value.");
            }

            err.InstructionError.BorshIoError = reader.GetString();
            reader.Read(); //string
            reader.Read(); //endobj
            reader.Read(); //endarray

            return err;
        }

        public override void Write(Utf8JsonWriter writer, TransactionError value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}