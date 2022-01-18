using Solnet.Rpc.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solnet.Rpc.Converters
{
    /// <inheritdoc/>
    public class EncodingConverter : JsonConverter<BinaryEncoding>
    {
        /// <inheritdoc/>
        public override BinaryEncoding Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void Write(Utf8JsonWriter writer, BinaryEncoding value, JsonSerializerOptions options)
        {
            if(value == BinaryEncoding.JsonParsed)
            {
                writer.WriteStringValue("jsonParsed");
            }
            else if (value == BinaryEncoding.Base64Zstd)
            {
                writer.WriteStringValue("base64+zstd");
            }
            else
            {
                writer.WriteStringValue("base64");
            }
        }
    }
}
