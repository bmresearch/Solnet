using System;

namespace Solnet.KeyStore.Exceptions
{
    public class DecryptionException : Exception
    {
        internal DecryptionException(string msg) : base(msg)
        {
        }
    }
}