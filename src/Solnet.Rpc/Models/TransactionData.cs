using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents the transaction, metadata and its containing slot.
    /// </summary>
    public class TransactionMetaSlotInfo : TransactionMetaInfo
    {
        /// <summary>
        /// The slot this transaction was processed in.
        /// </summary>
        public ulong Slot { get; set; }

        /// <summary>
        /// Estimated block production time.
        /// </summary>
        public long? BlockTime { get; set; }
    }


    /// <summary>
    /// Represents the tuple transaction and metadata.
    /// </summary>
    public class TransactionMetaInfo
    {
        /// <summary>
        /// The transaction information.
        /// </summary>  
        [JsonConverter(typeof(TransactionDataConverter))]
        public object Transaction { get; set; }

        /// <summary>
        /// The metadata information.
        /// </summary>
      
        public TransactionMeta Meta { get; set; }

        /// <summary>
        /// Transaction Version
        /// </summary>
        ///  [JsonPropertyName("value")]
        [JsonConverter(typeof(DynamicTypeConverter))]
        public object Version { get; set; }
    }
    /// <summary>
    /// Handles different transaction meta encodings when deserialized
    /// </summary>
    public class TransactionDataConverter : JsonConverter<object>
    {
        /// <summary>
        /// Read
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
            {
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    return doc.RootElement.Deserialize<TransactionInfo>(options);
                }
                else if (doc.RootElement.ValueKind == JsonValueKind.Array)
                {
                    var array = doc.RootElement;
                    bool isStringArray = true;
                    foreach (var element in array.EnumerateArray())
                    {
                        if (element.ValueKind != JsonValueKind.String)
                        {
                            isStringArray = false;
                            break;
                        }
                    }

                    if (isStringArray)
                    {
                        return array.Deserialize<string[]>(options);
                    }
                }
            }

            throw new JsonException("Unsupported JSON value type");
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    /// <summary>
    /// Json Converter for handling string and integer version types
    /// </summary>
    public class DynamicTypeConverter : JsonConverter<object>
    {

        /// <summary>
        /// Read
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="JsonException"></exception>
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return reader.GetString();
            }
            else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out int value))
            {
                return value;
            }

            throw new JsonException();
        }

        /// <summary>
        /// Write
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        /// <exception cref="JsonException"></exception>
        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value is int)
            {
                writer.WriteNumberValue((int)value);
            }
            else if (value is string)
            {
                writer.WriteStringValue((string)value);
            }
            else
            {
                throw new JsonException();
            }
        }
    }

    /// <summary>
    /// Represents a transaction.
    /// </summary>
    public class TransactionInfo
    {
        /// <summary>
        /// The signatures of this transaction.
        /// </summary>
        public string[] Signatures { get; set; }

        /// <summary>
        /// The message contents of the transaction.
        /// </summary>
        public TransactionContentInfo Message { get; set; }
    }

    /// <summary>
    /// Represents the contents of the trasaction.
    /// </summary>
    public class TransactionContentInfo
    {
        /// <summary>
        /// List of base-58 encoded public keys used by the transaction, including by the instructions and for signatures.
        /// </summary>
        public string[] AccountKeys { get; set; }

        /// <summary>
        /// Details the account types and signatures required by the transaction.
        /// </summary>
        public TransactionHeaderInfo Header { get; set; }

        /// <summary>
        ///  A base-58 encoded hash of a recent block in the ledger used to prevent transaction duplication and to give transactions lifetimes.
        /// </summary>
        public string RecentBlockhash { get; set; }

        /// <summary>
        /// List of program instructions that will be executed in sequence and committed in one atomic transaction if all succeed.
        /// </summary>
        public InstructionInfo[] Instructions { get; set; }
    }

    /// <summary>
    /// Details the number and type of accounts and signatures in a given transaction.
    /// </summary>
    public class TransactionHeaderInfo
    {
        /// <summary>
        /// The total number of signatures required to make the transaction valid. 
        /// </summary>
        public int NumRequiredSignatures { get; set; }

        /// <summary>
        /// The last NumReadonlySignedAccounts of the signed keys are read-only accounts.
        /// </summary>
        public int NumReadonlySignedAccounts { get; set; }

        /// <summary>
        /// The last NumReadonlyUnsignedAccounts of the unsigned keys are read-only accounts.
        /// </summary>
        public int NumReadonlyUnsignedAccounts { get; set; }
    }

    /// <summary>
    /// Represents the transaction metadata.
    /// </summary>
    public class TransactionMeta
    {
        /// <summary>
        /// Possible transaction error.
        /// </summary>
        [JsonPropertyName("err")]
        public TransactionError Error { get; set; }

        /// <summary>
        /// Fee this transaction was charged.
        /// </summary>
        public ulong Fee { get; set; }

        /// <summary>
        /// Collection of account balances from before the transaction was processed.
        /// </summary>
        public ulong[] PreBalances { get; set; }

        /// <summary>
        /// Collection of account balances after the transaction was processed.
        /// </summary>
        public ulong[] PostBalances { get; set; }

        /// <summary>
        /// List of inner instructions or omitted if inner instruction recording was not yet enabled during this transaction.
        /// </summary>
        public InnerInstruction[] InnerInstructions { get; set; }

        /// <summary>
        /// List of token balances from before the transaction was processed or omitted if token balance recording was not yet enabled during this transaction.
        /// </summary>
        public TokenBalanceInfo[] PreTokenBalances { get; set; }

        /// <summary>
        /// List of token balances from after the transaction was processed or omitted if token balance recording was not yet enabled during this transaction.
        /// </summary>
        public TokenBalanceInfo[] PostTokenBalances { get; set; }

        /// <summary>
        /// Array of string log messages or omitted if log message recording was not yet enabled during this transaction.
        /// </summary>
        public string[] LogMessages { get; set; }

        /// <summary>
        /// Transaction-level rewards, populated if rewards are requested
        /// </summary>
        public RewardInfo[] Rewards { get; set; }

        /// <summary>
        /// Transaction addresses loaded from address lookup tables.
        /// </summary>
   
        public loadedAddresses LoadedAddresses { get; set; }
    }   
    
    /// <summary>
    /// Represents an inner instruction. Inner instruction are cross-program instructions that are invoked during transaction processing.
    /// </summary>
    public class InnerInstruction
    {
        /// <summary>
        /// Index of the transaction instruction from which the inner instruction(s) originated
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// List of program instructions that will be executed in sequence and committed in one atomic transaction if all succeed.
        /// </summary>
        public InstructionInfo[] Instructions { get; set; }
    }

    /// <summary>
    /// Represents the data of given instruction.
    /// </summary>
    public class InstructionInfo
    {
        /// <summary>
        /// Index into the <i>Message.AccountKeys</i> array indicating the program account that executes this instruction.
        /// </summary>
        public int ProgramIdIndex { get; set; }

        /// <summary>
        /// List of ordered indices into the <i>Message.AccountKeys</i> array indicating which accounts to pass to the program.
        /// </summary>
        public int[] Accounts { get; set; }

        /// <summary>
        /// The program input data encoded in a base-58 string.
        /// </summary>
        public string Data { get; set; }
    }

    /// <summary>
    /// Represents the structure of a token balance metadata for a transaction.
    /// </summary>
    public class TokenBalanceInfo
    {
        /// <summary>
        /// Index of the account in which the token balance is provided for.
        /// </summary>
        public int AccountIndex { get; set; }

        /// <summary>
        /// Pubkey of the token's mint.
        /// </summary>
        public string Mint { get; set; }

        /// <summary>
        /// Pubkey of the token owner
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// Token balance details.
        /// </summary>
        public TokenBalance UiTokenAmount { get; set; }
    }

 
    /// <summary>
    /// Transaction addresses loaded from address lookup tables.
    /// </summary>
    public class loadedAddresses
    {
        /// <summary>
        /// Writable address list
        /// </summary>
        public string[] Writable { get; set; }

        /// <summary>
        /// Readonly address list
        /// </summary>
        public string[] Readonly { get; set; }
    }
}
