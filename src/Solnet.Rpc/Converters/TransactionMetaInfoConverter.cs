using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

using Solnet.Rpc.Models;

namespace Solnet.Rpc.Converters;

/// <summary>
/// Converts a TransactionMetaInfo from json into its model representation.
/// </summary>
public class TransactionMetaInfoConverter : JsonConverter<TransactionMetaInfo>
{
    /// <summary>
    /// Reads and converts the JSON to type <c>TransactionMetaInfo</c>.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="typeToConvert"> The type to convert.</param>
    /// <param name="options">An object that specifies serialization options to use.</param>
    /// <returns>The converted value.</returns>
    public override TransactionMetaInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using JsonDocument document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        // Get original account keys from transaction.message.accountKeys
        var accountKeys = root.GetProperty("transaction")
            .GetProperty("message")
            .GetProperty("accountKeys")
            .EnumerateArray()
            .Select(x => x.GetString())
            .ToList();

        // Get loaded addresses from meta.loadedAddresses if exists
        if (root.TryGetProperty("meta", out JsonElement meta) &&
            meta.TryGetProperty("loadedAddresses", out JsonElement loadedAddresses))
        {
            // Add writable addresses
            if (loadedAddresses.TryGetProperty("writable", out JsonElement writable))
            {
                accountKeys.AddRange(writable.EnumerateArray().Select(x => x.GetString()));
            }

            // Add readonly addresses
            if (loadedAddresses.TryGetProperty("readonly", out JsonElement readonly_))
            {
                accountKeys.AddRange(readonly_.EnumerateArray().Select(x => x.GetString()));
            }
        }

        // Create transaction with updated account keys
        var transaction = JsonSerializer.Deserialize<TransactionInfo>(root.GetProperty("transaction"), options);
        transaction.Message.AccountKeys = accountKeys.ToArray();
        // transaction = transaction with
        // {
        //     Message = transaction.Message with
        //     {
        //         AccountKeys = accountKeys.ToArray()
        //     }
        // };

        // Create meta info
        var txMeta = JsonSerializer.Deserialize<TransactionMeta>(root.GetProperty("meta"), options);

        return new TransactionMetaInfo
        {
            Transaction = transaction,
            Meta = txMeta
        };
    }

    /// <summary>
    /// Partially implemented.
    /// </summary>
    /// <param name="writer">n/a</param>
    /// <param name="value">n/a</param>
    /// <param name="options">n/a</param>
    public override void Write(Utf8JsonWriter writer, TransactionMetaInfo value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}