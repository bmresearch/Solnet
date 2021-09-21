using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solnet.Rpc.Core.Http
{
    /// <summary>
    /// Helper class that holds a key-value config pair that filters out null values.
    /// </summary>
    internal class KeyValue : Tuple<string, object>
    {
        private KeyValue(string item1, object item2) : base(item1, item2)
        {
        }

        internal static KeyValue Create(string key, object value)
        {
            if (value != null)
            {
                return new KeyValue(key, value);
            }
            return null;
        }
    }

    /// <summary>
    /// Helper class to create configuration objects with key-value pairs that filters out null values.
    /// </summary>
    internal static class ConfigObject
    {
        internal static Dictionary<string, object> Create(KeyValue pair1)
        {
            if (pair1 != null)
            {
                return new Dictionary<string, object> { { pair1.Item1, pair1.Item2 } };
            }
            return null;
        }

        internal static Dictionary<string, object> Create(KeyValue pair1, KeyValue pair2)
        {
            var dict = Create(pair1) ?? new Dictionary<string, object>();

            if (pair2 != null)
            {
                dict.Add(pair2.Item1, pair2.Item2);
            }

            return dict.Count > 0 ? dict : null;
        }

        internal static Dictionary<string, object> Create(KeyValue pair1, KeyValue pair2, KeyValue pair3)
        {
            var dict = Create(pair1, pair2) ?? new Dictionary<string, object>();

            if (pair3 != null)
            {
                dict.Add(pair3.Item1, pair3.Item2);
            }

            return dict.Count > 0 ? dict : null;
        }

        internal static Dictionary<string, object> Create(KeyValue pair1, KeyValue pair2, KeyValue pair3, KeyValue pair4)
        {
            var dict = Create(pair1, pair2, pair3) ?? new Dictionary<string, object>();

            if (pair4 != null)
            {
                dict.Add(pair4.Item1, pair4.Item2);
            }

            return dict.Count > 0 ? dict : null;
        }

        internal static Dictionary<string, object> Create(KeyValue pair1, KeyValue pair2, KeyValue pair3, KeyValue pair4, KeyValue pair5)
        {
            var dict = Create(pair1, pair2, pair3, pair4) ?? new Dictionary<string, object>();

            if (pair5 != null)
            {
                dict.Add(pair5.Item1, pair5.Item2);
            }

            return dict.Count > 0 ? dict : null;
        }
    }

    /// <summary>
    /// Helper class that creates a List of parameters and filters out null values.
    /// </summary>
    internal static class Parameters
    {
        internal static List<object> Create(object val1)
        {
            if (val1 != null)
            {
                return new List<object> { val1 };
            }
            return null;
        }

        internal static List<object> Create(object val1, object val2)
        {
            var list = Create(val1) ?? new List<object>();
            if (val2 != null)
            {
                list.Add(val2);
            }
            return list.Count > 0 ? list : null;
        }

        internal static List<object> Create(object val1, object val2, object val3)
        {
            var list = Create(val1, val2) ?? new List<object>();
            if (val3 != null)
            {
                list.Add(val3);
            }
            return list.Count > 0 ? list : null;
        }
    }
}