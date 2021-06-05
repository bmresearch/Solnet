using Solnet.KeyStore.Exceptions;
using Solnet.KeyStore.Model;
using Solnet.KeyStore.Services;
using System;
using System.Runtime.Serialization;
using System.Text.Json;

namespace Solnet.KeyStore
{
    /// <summary>
    /// Implements a checker for the <see cref="KeyStore{TKdfParams}"/>'s <see cref="KdfType"/>.
    /// </summary>
    public static class KeyStoreKdfChecker
    {
        /// <summary>
        /// Get the kdf type string from the json document.
        /// </summary>
        /// <param name="keyStoreDocument">The json document.</param>
        /// <returns>The kdf type string.</returns>
        /// <exception cref="JsonException">Throws exception when json property <c>crypto</c> or <c>kdf</c> couldn't be found</exception>
        private static string GetKdfTypeFromJson(JsonDocument keyStoreDocument)
        {
            var cryptoObjExist = keyStoreDocument.RootElement.TryGetProperty("crypto", out var cryptoObj);
            if (!cryptoObjExist) throw new JsonException("could not get crypto params object from json");

            var kdfObjExist = cryptoObj.TryGetProperty("kdf", out var kdfObj);
            if (!kdfObjExist) throw new JsonException("could not get kdf object from json");

            return kdfObj.GetString();
        }

        /// <summary>
        /// Gets the kdf type from the json keystore.
        /// </summary>
        /// <param name="json">The json keystore.</param>
        /// <returns>The kdf type.</returns>
        /// <exception cref="ArgumentNullException">Throws exception when <c>json</c> param is null.</exception>
        /// <exception cref="SerializationException">Throws exception when file could not be processed to <see cref="JsonDocument"/>.</exception>
        /// <exception cref="JsonException">Throws exception when <c>kdf</c> json property is <c>null</c>.</exception>
        /// <exception cref="InvalidKdfException">Throws exception when the <c>kdf</c> json property has an invalid <see cref="KdfType"/> value.</exception>
        public static KdfType GetKeyStoreKdfType(string json)
        {
            if (json == null) throw new ArgumentNullException(nameof(json));
            var keyStoreDocument = JsonSerializer.Deserialize<JsonDocument>(json);
            if (keyStoreDocument == null) throw new SerializationException("could not process json");

            var kdfString = GetKdfTypeFromJson(keyStoreDocument);

            if (kdfString == null) throw new JsonException("could not get kdf type from json");
            return kdfString switch
            {
                KeyStorePbkdf2Service.KdfType => KdfType.Pbkdf2,
                KeyStoreScryptService.KdfType => KdfType.Scrypt,
                _ => throw new InvalidKdfException(kdfString)
            };
        }
    }
}