
namespace Solnet.Wallet.Utilities
{
    public abstract class DataEncoder
    {
        // char.IsWhiteSpace fits well but it match other whitespaces
        // characters too and also works for unicode characters.
        public static bool IsSpace(char c)
        {
            switch (c)
            {
                case ' ':
                case '\t':
                case '\n':
                case '\v':
                case '\f':
                case '\r':
                    return true;
            }
            return false;
        }

        internal DataEncoder()
        {
        }

        public string EncodeData(byte[] data)
        {
            return EncodeData(data, 0, data.Length);
        }

        public abstract string EncodeData(byte[] data, int offset, int count);


        public abstract byte[] DecodeData(string encoded);
    }

    public static class Encoders
    {
        private static readonly Base58Encoder _Base58 = new();
        public static DataEncoder Base58 => _Base58;
    }
}