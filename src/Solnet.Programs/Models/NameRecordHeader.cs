using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Solnet.Programs.Utilities;

namespace Solnet.Programs.Models
{
    //public class NameRecord<T> : NameRecord
    //{

    //    public T Record { get; set; }

    //    public static NameRecord<T> Deserialize<T>(byte[] input)
    //    {
    //        var data = new ReadOnlySpan<byte>(input);

    //        var res = new NameRecord<T>();

    //        res.ParentName = data.GetPubKey(0);
    //        res.Owner = data.GetPubKey(32);
    //        res.Class = data.GetPubKey(64);

    //        System.Reflection.MethodInfo m = typeof(T).GetMethod("Deserialize",
    //            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
    //            null, new[] { typeof(byte[]) }, null);

    //        if (m != null)
    //            res.Record = (T)m.Invoke(null, new object[] { input });

    //        return res;
    //    }
    //}

    //public class ReverseTwitterRecord
    //{
    //    public PublicKey TwitterRegistryKey { get; set; }

    //    public string TwitterHandle { get; set; }

    //    public static ReverseTwitterRecord Deserialize(byte[] input)
    //    {
    //        var data = new ReadOnlySpan<byte>(input, 96, input.Length - 96);
    //        var ret = new ReverseTwitterRecord();

    //        ret.TwitterRegistryKey = data.GetPubKey(0);
    //        _ = data.GetString(32, out var str);
    //        ret.TwitterHandle = str;

    //        return ret;
    //    }
    //}

    public class RecordHeader
    {
        public PublicKey ParentName { get; set; }
        public PublicKey Owner { get; set; }
        public PublicKey Class { get; set; }

        public static RecordHeader Deserialize(byte[] input)
        {
            var data = new ReadOnlySpan<byte>(input);

            var res = new RecordHeader();

            res.ParentName = data.GetPubKey(0);
            res.Owner = data.GetPubKey(32);
            res.Class = data.GetPubKey(64);

            return res;
        }
    }

    public enum RecordType
    {
        NameRecord,
        ReverseRecord,
        TwitterRecord,
        ReverseTwitterRecord,
        TokenRecord,
        ReverseTokenRecord
    }


    //public class Record<T> : Record
    //{
    //    public override object GetValue() => Value;

    //    public T Value { get; set; }

    //    public static Record<T> Deserialize(byte[] input)
    //    {
    //        var data = new ReadOnlySpan<byte>(input);
    //        var header = DeserializeHeader(data);


    //        var res = new Record<T>();

    //        if(typeof(T).Equals(typeof(string)))
    //        {
    //            _ = data.GetString(0, out var str);
    //            res.Value = str;
    //        }

    //        return res;
    //    }

    //    private static RecordHeader DeserializeHeader(ReadOnlySpan<byte> input)
    //    {
    //        var header = new RecordHeader();

    //        header.ParentName = input.GetPubKey(0);
    //        header.Owner = input.GetPubKey(32);
    //        header.Class = input.GetPubKey(64);

    //        return header;
    //    }

    //    public static Record Deserialize(byte[] input)
    //    {
    //        var data = new ReadOnlySpan<byte>(input);
    //        var header = DeserializeHeader(data);


    //    }
    //}
}
