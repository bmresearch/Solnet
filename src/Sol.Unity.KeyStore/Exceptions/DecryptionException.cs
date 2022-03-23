using System;

namespace Sol.Unity.KeyStore.Exceptions
{
    public class DecryptionException : Exception
    {
        internal DecryptionException(string msg) : base(msg)
        {
        }
    }
}