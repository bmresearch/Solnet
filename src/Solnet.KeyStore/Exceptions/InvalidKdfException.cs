using System;

namespace Solnet.KeyStore.Exceptions
{
    public class InvalidKdfException : Exception
    {
        public InvalidKdfException(string kdf) : base("Invalid kdf:" + kdf)
        {
        }
    }
}