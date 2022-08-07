using Solnet.Rpc.Models;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Converters
{
    /// <summary>
    /// Converts a TransactionError from json into its model representation.
    /// </summary>
    public class TransactionErrorJsonConverter : JsonConverter<TransactionError>
    {
        /// <summary>
        /// Reads and converts the JSON to type <c>TransactionError</c>.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="typeToConvert"> The type to convert.</param>
        /// <param name="options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        public override TransactionError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null) return null;

            var err = new TransactionError();

            if (reader.TokenType == JsonTokenType.String)
            {
                var enumValue = reader.GetString();

                Enum.TryParse(enumValue, ignoreCase: false, out TransactionErrorType errorType);
                err.Type = errorType;
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
                {
                    var enumValue = reader.GetString();
                    Enum.TryParse(enumValue, ignoreCase: false, out TransactionErrorType errorType);
                    err.Type = errorType;
                }

                if (err.Type == TransactionErrorType.InstructionError)
                {
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
                        err.InstructionError.CustomError = reader.GetUInt32();
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
                }
                else
                {
                    //TODO: should we modify transaction error to include error details for complex error type such as DuplicateInstruction or InsufficientFundsForRent?
                    reader.Read(); //startobj details
                    reader.Read(); //details property name
                    reader.Read(); //details property value
                    reader.Read(); //endobj details
                    reader.Read(); //endobj
                    return err;
                }
            }

            return err;
        }

        /// <summary>
        /// Partially implemented.
        /// </summary>
        /// <param name="writer">n/a</param>
        /// <param name="value">n/a</param>
        /// <param name="options">n/a</param>
        public override void Write(Utf8JsonWriter writer, TransactionError value, JsonSerializerOptions options)
        {
            if (value.InstructionError != null)
            {

                // looking to output something like this...
                // { 'InstructionError': [0, 'InvalidAccountData'] }
                writer.WriteStartObject();
                writer.WritePropertyName("InstructionError");

                // innards
                var enumName = value.InstructionError.Type.ToString();
                writer.WriteStartArray();
                writer.WriteNumberValue(value.InstructionError.InstructionIndex);
                writer.WriteStringValue(enumName);
                writer.WriteEndArray();

                writer.WriteEndObject();

            }
            else
                throw new NotImplementedException();
        }
    }
}