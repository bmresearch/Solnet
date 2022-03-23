using System;

namespace Sol.Unity.KeyStore.Exceptions
{
    public class InvalidKdfException : Exception
    {
        public InvalidKdfException(string kdf) : base("Invalid kdf:" + kdf)
        {
        }
    }
}