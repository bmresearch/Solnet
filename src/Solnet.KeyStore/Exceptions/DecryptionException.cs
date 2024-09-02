#pragma warning disable CS1591
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