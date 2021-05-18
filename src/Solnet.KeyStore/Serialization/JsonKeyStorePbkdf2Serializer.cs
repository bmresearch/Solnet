using Newtonsoft.Json;
using Solnet.KeyStore.Model;

namespace Solnet.KeyStore.Serialization
{
    public class JsonKeyStorePbkdf2Serialiser
    {
        public static string SerialisePbkdf2(KeyStore<Pbkdf2Params> pbdk2KeyStore)
        {
            var dto = MapModelToDto(pbdk2KeyStore);
            return JsonConvert.SerializeObject(pbdk2KeyStore);
        }

        public static KeyStore<Pbkdf2Params> DeserialisePbkdf2(string json)
        {
            var dto = JsonConvert.DeserializeObject<KeyStorePbkdf2Dto>(json);
            return MapDtoToModel(dto);
        }

        public static KeyStorePbkdf2Dto MapModelToDto(KeyStore<Pbkdf2Params> pbdk2KeyStore)
        {
            var dto = new KeyStorePbkdf2Dto();
            dto.address = pbdk2KeyStore.Address;
            dto.id = pbdk2KeyStore.Id;
            dto.version = pbdk2KeyStore.Version;
            dto.crypto.cipher = pbdk2KeyStore.Crypto.Cipher;
            dto.crypto.cipherText = pbdk2KeyStore.Crypto.CipherText;
            dto.crypto.kdf = pbdk2KeyStore.Crypto.Kdf;
            dto.crypto.mac = pbdk2KeyStore.Crypto.Mac;
            dto.crypto.kdfparams.c = pbdk2KeyStore.Crypto.Kdfparams.Count;
            dto.crypto.kdfparams.prf = pbdk2KeyStore.Crypto.Kdfparams.Prf;
            dto.crypto.kdfparams.dklen = pbdk2KeyStore.Crypto.Kdfparams.Dklen;
            dto.crypto.kdfparams.salt = pbdk2KeyStore.Crypto.Kdfparams.Salt;
            dto.crypto.cipherparams.iv = pbdk2KeyStore.Crypto.CipherParams.Iv;
            return dto;
        }

        public static KeyStore<Pbkdf2Params> MapDtoToModel(KeyStorePbkdf2Dto dto)
        {
            var pbdk2KeyStore = new KeyStore<Pbkdf2Params>();
            pbdk2KeyStore.Address = dto.address;
            pbdk2KeyStore.Id = dto.id;
            pbdk2KeyStore.Version = dto.version;
            pbdk2KeyStore.Crypto = new CryptoInfo<Pbkdf2Params>();
            pbdk2KeyStore.Crypto.Cipher = dto.crypto.cipher;
            pbdk2KeyStore.Crypto.CipherText = dto.crypto.cipherText;
            pbdk2KeyStore.Crypto.Kdf = dto.crypto.kdf;
            pbdk2KeyStore.Crypto.Mac = dto.crypto.mac;
            pbdk2KeyStore.Crypto.Kdfparams = new Pbkdf2Params();
            pbdk2KeyStore.Crypto.Kdfparams.Count = dto.crypto.kdfparams.c;
            pbdk2KeyStore.Crypto.Kdfparams.Prf = dto.crypto.kdfparams.prf;
            pbdk2KeyStore.Crypto.Kdfparams.Dklen = dto.crypto.kdfparams.dklen;
            pbdk2KeyStore.Crypto.Kdfparams.Salt = dto.crypto.kdfparams.salt;
            pbdk2KeyStore.Crypto.CipherParams = new CipherParams();
            pbdk2KeyStore.Crypto.CipherParams.Iv = dto.crypto.cipherparams.iv;
            return pbdk2KeyStore;
        }
    }
}