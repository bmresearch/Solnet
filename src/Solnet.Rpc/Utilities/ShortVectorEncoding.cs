using System;

namespace Solnet.Rpc.Utilities
{

    public static class ShortVectorEncoding
    {
        public static byte[] EncodeLength(int len) {
            var output = new byte[10];
            var remLen = len;
            var cursor = 0;

            for (;;) {
                var elem = remLen & 0x7f;
                remLen >>= 7;
                if (remLen == 0)
                {
                    output[cursor] = (byte) elem;
                    break;
                }
                elem |= 0x80;
                output[cursor] = (byte) elem;
                cursor += 1;
            }

            var bytes = new byte[cursor + 1];
            Array.Copy(output, 0, bytes, 0, cursor + 1);

            return bytes;
        }
    }
}