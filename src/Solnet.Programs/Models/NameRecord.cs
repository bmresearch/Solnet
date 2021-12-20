using Solnet.Wallet;
using System;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.Models
{

    public class ReverseTwitterRecord : RecordBase
    {
        public ReverseTwitterRecord(RecordHeader header, RecordType type) : base(header, type)
        {
        }
        public PublicKey TwitterRegistryKey { get; set; }

        public string TwitterHandle { get; set; }

        public override object GetValue() => TwitterHandle;

        public static ReverseTwitterRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);
            var header = RecordHeader.Deserialize(input);

            var ret = new ReverseTwitterRecord(header, RecordType.ReverseTwitterRecord);

            ret.TwitterRegistryKey = data.GetPubKey(0);
            _ = data.GetString(32, out var str);
            ret.TwitterHandle = str;

            return ret;
        }
    }

    public class TokenNameRecord : RecordBase
    {
        public TokenNameRecord(RecordHeader header, RecordType type) : base(header, type)
        {
        }

        public TokenData Value { get; set; }

        public override object GetValue() => Value;

        public static TokenNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);

            var header = RecordHeader.Deserialize(input);

            var res = new TokenNameRecord(header, RecordType.TokenRecord);

            res.Value = TokenData.Deserialize(input);

            return res;
        }
    }


    public class ReverseTokenNameRecord : RecordBase
    {
        public ReverseTokenNameRecord(RecordHeader header, RecordType type) : base(header, type)
        {
        }

        public PublicKey Value { get; set; }

        public override object GetValue() => Value;

        public static ReverseTokenNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);

            var header = RecordHeader.Deserialize(input);

            var res = new ReverseTokenNameRecord(header, RecordType.ReverseTokenRecord);

            res.Value = data.GetPubKey(96);

            return res;
        }
    }

    public class NameRecord : RecordBase
    {
        public NameRecord(RecordHeader header, RecordType type) : base(header, type)
        {
        }

        public byte[] Value { get; set; }

        public override object GetValue() => Value;

        public static NameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);

            var header = RecordHeader.Deserialize(input);

            var res = new NameRecord(header, RecordType.NameRecord);

            res.Value = input[96..];

            return res;
        }
    }

    public class ReverseNameRecord : RecordBase
    {
        public ReverseNameRecord(RecordHeader header, RecordType type) : base(header, type)
        {
        }

        public string Name { get; set; }

        public override object GetValue() => Name;

        public NameRecord Value { get; internal set; }

        public static ReverseNameRecord Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input, 96, input.Length-96);

            var header = RecordHeader.Deserialize(input);

            var res = new ReverseNameRecord(header, RecordType.ReverseRecord);

            _ = data.GetString(0, out var str);
            res.Name = str;

            return res;
        }
    }

    public abstract class RecordBase
    {
        public RecordHeader Header { get; }

        public RecordType Type { get; }

        public PublicKey Address { get; set; }

        public RecordBase(RecordHeader header, RecordType type)
        {
            Header = header;
            Type = type;
        }

        public abstract object GetValue();
    }
}
