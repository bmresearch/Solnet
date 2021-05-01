using System;
using Newtonsoft.Json.Linq;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements functionality to check the keystore's key derivation function.
    /// </summary>
    public class KeyStoreKdfChecker
    {

        public KdfType GetKeyStoreKdfType(string json)
        {
            try
            {
                var keyStoreDocument = JObject.Parse(json);
                var kdf = keyStoreDocument.GetValue("crypto", StringComparison.OrdinalIgnoreCase)["kdf"]
                    .Value<string>();

                if (kdf == KeyStorePbkdf2Service.KdfType)
                    return KdfType.pbkdf2;

                if (kdf == KeyStoreScryptService.KdfType)
                    return KdfType.scrypt;
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid KeyStore json", ex);
            }
        }
        
    }
}