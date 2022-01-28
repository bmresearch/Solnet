using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class InstructionDecoderTest
    {
        private const string Base64Message =
            "AgAEBmeEU5GowlV7Ug3Y0gjKv+31fvJ5iq+FC+pj+blJfEu615Bs5Vo6mnXZXvh35ULmThtyhwH8xzD" +
            "k8CgGqB1ISymLH0tOe6K/10n8jVYmg9CCzfFJ7Q/PtKWCWZjI/MJBiQan1RcZLFxRIYzJTD1K8X9Y2u4I" +
            "m6H9ROPb2YoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAG3fbh12Whk9nL4UbO63msHL" +
            "SF7V9bN5E6jPWFfv8AqeIfQzb6ERv8S2AqP3kpqFe1rhOi8a8q+HoB5Z/4WUfiAgQCAAE0AAAAAPAdHwAA" +
            "AAAApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUEAQIAAwEB";

        private const string CreateNameRecordMessage =
            "AQAEB0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey2F2FblAWiu/ZiWbtqWN8z7Ge/AluZCaFnLAg" +
            "KgEtFkwslVgjMhgItKaUceGeZWs2nDDtcND1HZzSINSX4+RUowAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAA7aYX6pwktdoNpXyPx7Nh0YfhYfKpR+3sTN7r+G2scDYLrVH0E8HzqZRg2QDYvy7Wkn7KNNe3hCv4EK" +
            "lzCC0e3AVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagP41xOPVGLLgO6dyCJM7tYzyOfSCB+M0/p3" +
            "dKUaQLAFcDBQYDAAEEAwMxACAAAACnNP3UHhvIIfmP8QPWH6ZQemEki1lon8kht12iPvr1LGCzGQAAAAAASA" +
            "QAAAUGAwACBAMDMQAgAAAAXu2y8Qu3cU5iu0O7no/eibVJ78FSE55pqFKBMdKeBGlgsxkAAAAAAHIAAAAGAQA" +
            "SSGVsbG8gZnJvbSBTb2wuTmV0";

        private const string CreateNameRecordMessageWithOptionals =
            "AgEGCkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeycGHk8ikVG/j9JQzbTKkpWBlW+vpDlDWt8piCH" +
            "h0e7gDuaLf4pEja2uB6kTRB1htLbUXXhJrJYf+30ka4JZOP5iyVWCMyGAi0ppRx4Z5lazacMO1w0PUdnNIg1J" +
            "fj5FSjAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADtphfqnCS12g2lfI/Hs2HRh+Fh8qlH7exM3uv" +
            "4baxwNrCYPdmMRF/CLi8dVaKd9oKhioURCE5uy8E47nB5XO+IVXbTe2oZLGbWE7gi8G/EzLa0LF+8Ue9zeKJS" +
            "XJSPQBALrVH0E8HzqZRg2QDYvy7Wkn7KNNe3hCv4EKlzCC0e3AVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniIS" +
            "TL07vagyktAe2blpkkd4ehXUaqEVj4wmgw65sDRc7iWHP5oLK4DCAcEAAIFAQYHMQAgAAAApzT91B4byCH5j/E" +
            "D1h+mUHphJItZaJ/JIbddoj769SxgsxkAAAAAAEgEAAAIBgQAAwUEBDEAIAAAAF7tsvELt3FOYrtDu56P3om1S" +
            "e/BUhOeaahSgTHSngRpYLMZAAAAAAByAAAACQEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string UpdateNameRegistryMessage =
            "AwIDCEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey7aYX6pwktdoNpXyPx7Nh0YfhYfKpR+3sTN7r+G2" +
            "scDZwYeTyKRUb+P0lDNtMqSlYGVb6+kOUNa3ymIIeHR7uANhdhW5QForv2Ylm7aljfM+xnvwJbmQmhZywICoBLR" +
            "ZMtPAsG74d79A7P/+E08SaNuYf6YBbdSOgY/77m55tHIgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AutUfQTwfOplGDZANi/LtaSfso017eEK/gQqXMILR7cBUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qDaw" +
            "66DgcyHvhNixl1hkJoZfTbA5s/xSGuMoKcGoAyVcgMGBgUAAwEFBTEAIAAAAKc0/dQeG8gh+Y/xA9YfplB6YSSL" +
            "WWifySG3XaI++vUsYLMZAAAAAABIBAAABgMEAQIJAX0AAAAAAAEBBwEAEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string TransferNameRegistryMessage =
            "AwIDCEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey7aYX6pwktdoNpXyPx7Nh0YfhYfKpR+3sTN7r+G2s" +
            "cDZwYeTyKRUb+P0lDNtMqSlYGVb6+kOUNa3ymIIeHR7uANhdhW5QForv2Ylm7aljfM+xnvwJbmQmhZywICoBLRZM" +
            "tPAsG74d79A7P/+E08SaNuYf6YBbdSOgY/77m55tHIgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAut" +
            "UfQTwfOplGDZANi/LtaSfso017eEK/gQqXMILR7cBUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qAx8BrK" +
            "IdZxPlzAvXk48kXy6OCV41zjcNSohay3j3WdgwMGBgUAAwEFBTEAIAAAAKc0/dQeG8gh+Y/xA9YfplB6YSSLWWify" +
            "SG3XaI++vUsYLMZAAAAAABIBAAABgMEAQIhAniwCzV/h6uUL2KYwVK8XBOnNTjxbbjrFk7EcGdtiUzbBwEAEkhlbG" +
            "xvIGZyb20gU29sLk5ldA==";

        private const string DeleteNameRegistryMessage =
            "AgEDCEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey7aYX6pwktdoNpXyPx7Nh0YfhYfKpR+3sTN7r+G2s" +
            "cDbYXYVuUBaK79mJZu2pY3zPsZ78CW5kJoWcsCAqAS0WTLTwLBu+He/QOz//hNPEmjbmH+mAW3UjoGP++5uebRyIc" +
            "GHk8ikVG/j9JQzbTKkpWBlW+vpDlDWt8piCHh0e7gAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAutUf" +
            "QTwfOplGDZANi/LtaSfso017eEK/gQqXMILR7cBUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qA928tpUmqf" +
            "Lt7plaOsM7DxFN9u5HEbr6SwhcgyXThQywMGBgUAAgEFBTEAIAAAAKc0/dQeG8gh+Y/xA9YfplB6YSSLWWifySG3Xa" +
            "I++vUsYLMZAAAAAABIBAAABgMDAQQBAwcBABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string UnknownInstructionMessage =
            "AwEGCUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyCPOc5WStiVWB4ReLWRVhjoAuppEeHwUSMtbx8Hmno" +
            "KY5g1hGR0SDr+x4hAd1OcuUEXP1Qyz3cU0b269EfBZb0gAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAie" +
            "4Ib1GlNzTEd9tj6EsaSwCA+dBgbKr3clv2+RhHVDMGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uH" +
            "XZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCp99LcpEIowBKqPubkZpgpqc6op2m6ZVvkvRXPi79K+JMFSlNQ+F3I" +
            "gtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oNgehyYY23GSdVDMiMrfxgbHc/HskbbAJqVQk2Dp67h1BAMCAAE0AAAAAP" +
            "AdHwAAAAAApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQYEAQQABQEBBwEABQEADxlNCAE" +
            "CEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string SharedMemoryWriteMessage =
            "AwEGCkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyCPOc5WStiVWB4ReLWRVhjoAuppEeHwUSMtbx8Hmno" +
            "KY5g1hGR0SDr+x4hAd1OcuUEXP1Qyz3cU0b269EfBZb0vfS3KRCKMASqj7m5GaYKanOqKdpumVb5L0Vz4u/SviTAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACJ7ghvUaU3NMR322PoSxpLAID50GBsqvdyW/b5GEdUMwan1Rc" +
            "ZLFxRIYzJTD1K8X9Y2u4Im6H9ROPb2YoAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkM/SOZwBqi" +
            "laVYsFyh41xY7e10qTG6fOOim2gK/x/4MQVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vag3QptoQFT2idlV" +
            "hj784S1Z6y6aq7mtGY0w6DA4CNAT+8EBAIAATQAAAAA8B0fAAAAAAClAAAAAAAAAAbd9uHXZaGT2cvhRs7reawctI" +
            "XtX1s3kTqM9YV+/wCpBwQBBQAGAQEIAQMNIwAAAAAAAAABAA8ZTQkBAhJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string DurableNonceMessage =
            "AQACBUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyhNzOq+Q0cJXarsJajrlwwzlmWoF5mx5wFN8OQ4OOJK" +
            "Lf9OU4VvMASlY6OI4RgnGTPQGIfvMW4q1sStRoUcd4tAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABqfV" +
            "FxksVo7gioRfc9KXiM8DXDFFshqzRNgGLqlAAACZ4OYEN7QEC8ChfqU50z8BgjxTJ0SwSF/AQXoalEjsRgIDAwIEAA" +
            "QEAAAAAwIAAQwCAAAAAMqaOwAAAAA=";

        private const string CreateWithSeedTransferCheckedMessage =
            "AQAFCEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyOYMO0iFs4aMUVosQrrL+aWspebSXbUiMaf5/Vser1b0OnC1i7fbauPEwr4QPwO60eHE6R2A3RGXr8HuhWwwwbgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA9LBeJnW7+BRnPyXA+KjPgyVxNJPg4ZjZj7vDCLaxcn6cN8HIDWf0F04DfWvktjd8c9zUrzgeo+yKgZUYC424xwan1RcZLFxRIYzJTD1K8X9Y2u4Im6H9ROPb2YoAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKm2aEncf4Mlb+sGgWJlGolxMb+4adawnHuBSBv1aK+CtQMDAgABZQMAAABHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgkAAAAAAAAAU29tZSBTZWVk8B0fAAAAAAClAAAAAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpBwQBBAUGAQEHBAIEAQAKDBAnAAAAAAAACg==";

        private const string AllocateAndTransferWithSeedMessage =
            "AwIECUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyxlZoHQB4RdUzPilsIbwW5CqatIYqbsEwlDlAxlbUberN+w3TZpSpkz6ceiNiFJ1YljgbSt+oGaN4XwsDKrjvO9eJ2GvItXyYvkkNtswujQh/3uFPx4eYNYHvmFKNj2KF6Nz9cBhJOumlXLZpUvE8AzAtBfGMn1dZQnsmstBxblEGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFSlNQ+F3IgtYUpVZyeIopbd8eq6vQpgZ4iEky9O72oIgZj6RKWuBs9/ZF9SblFNX1Nfndq/bZbd1zKevX07NqBgYCAwVDAAJHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgFHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgcCAQRdCQAAAOjc/XAYSTrppVy2aVLxPAMwLQXxjJ9XWUJ7JrLQcW5RCQAAAAAAAABTb21lIFNlZWSlAAAAAAAAAP4AAajK+Dt7AHzpNyQpMLfMDxb3r2T1UUbVQedlcfEvBwMEAQE9CwAAAKhhAAAAAAAACQAAAAAAAABTb21lIFNlZWRHaauXIEuoP7DK7hf3ho8eB05SFYGg2J2UN52qZbcXsgYEAgMABQEBBgMDAgAJB0BCDwAAAAAACAECEkhlbGxvIGZyb20gU29sLk5ldA==";

        private const string AssignWithSeedAndWithdrawNonceMessage =
            "AgEFCkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyzfsN02aUqZM+nHojYhSdWJY4G0rfqBmjeF8LAyq47zvXidhryLV8mL5JDbbMLo0If97hT8eHmDWB75hSjY9ihcZWaB0AeEXVMz4pbCG8FuQqmrSGKm7BMJQ5QMZW1G3q6Nz9cBhJOumlXLZpUvE8AzAtBfGMn1dZQnsmstBxblEGp9UXGSxcUSGMyUw9SvF/WNruCJuh/UTj29mKAAAAAAbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAGp9UXGSxWjuCKhF9z0peIzwNcMUWyGrNE2AYuqUAAAAVKU1D4XciC1hSlVnJ4iilt3x6rq9CmBniISTL07vagtJ8Jx8NOgvPxbiEudqErtkdKNjEMCpOGKmW34JXG2P8GBgICBUMAAkdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyAUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyBwIDBFUKAAAA6Nz9cBhJOumlXLZpUvE8AzAtBfGMn1dZQnsmstBxblEJAAAAAAAAAFNvbWUgU2VlZP4AAajK+Dt7AHzpNyQpMLfMDxb3r2T1UUbVQedlcfEvBwUEAwgFAAwFAAAAqGEAAAAAAAAGBAECAAUBAQYDAgEACQdAQg8AAAAAAAkBARJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string CreateNonceAccountMessage =
            "AgADBUdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey3/TlOFbzAEpWOjiOEYJxkz0BiH7zFuKtbErUaFHHeLQA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAan1RcZLFaO4IqEX3PSl4jPA1wxRbIas0TYBi6pQAAABqfVFxk" +
            "sXFEhjMlMPUrxf1ja7gibof1E49vZigAAAACHEetpR5UtsSacYYjH7rp2SZreGmXDVinNPeuZO1XQ8AICAgABNAAAAAA" +
            "AFxYAAAAAAFAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACAwEDBCQGAAAAR2mrlyBLqD+wyu4" +
            "X94aPHgdOUhWBoNidlDedqmW3F7I=";

        private const string AuthorizeNonceAccountMessage =
            "AQABA0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxey3/TlOFbzAEpWOjiOEYJxkz0BiH7zFuKtbErUaFHHeLQA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAkF38bO8K2XOUFDq7VOkCaRObsKUZyPb587Rcoo4eivAQICAQAkB" +
            "wAAACqCAIOtweetcVDQTjbgtE+ULaVRy1/RIR5APIhz/3J6";

        [TestMethod]
        public void InstructionDecoderRegisterTest()
        {
            InstructionDecoder.Register(new PublicKey("11111111111111111111111111111112"), (_, _, _) => new DecodedInstruction());

            Assert.IsNotNull(InstructionDecoder.Decode(new PublicKey("11111111111111111111111111111112"), Array.Empty<byte>(),
                new List<PublicKey>(), Array.Empty<byte>()));
        }

        [TestMethod]
        public void InstructionDecoderRegisterNullTest()
        {
            InstructionDecoder.Register(new PublicKey("11111111111111111111111111111122"), (_, _, _) => new DecodedInstruction());

            Assert.IsNull(InstructionDecoder.Decode(new PublicKey("11111111111111111111111111111123"), Array.Empty<byte>(),
                new List<PublicKey>(), Array.Empty<byte>()));
        }

        [TestMethod]
        public void DecodeInstructionsFromMessageTest()
        {
            Message msg = Message.Deserialize(Base64Message);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);

            Assert.AreEqual("Create Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner Account", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Account", out object newAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("7y62LXLwANaN9g3KJPxQFYwMxSdZraw5PkqwtqY9zLDF", (PublicKey)owner);
            Assert.AreEqual("FWUPMzrLbAEuH83cf1QphoFdyUdhenDF5oHftwd9Vjyr", (PublicKey)newAccount);
            Assert.AreEqual(2039280UL, (ulong)amount);
            Assert.AreEqual(165UL, (ulong)space);

            Assert.AreEqual("Initialize Account", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out owner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual("FWUPMzrLbAEuH83cf1QphoFdyUdhenDF5oHftwd9Vjyr", (PublicKey)account);
            Assert.AreEqual("AN5M7KvEFiZFxgEUWFdZUdR5i4b96HjXawADpqjxjXCL", (PublicKey)mint);
            Assert.AreEqual("7y62LXLwANaN9g3KJPxQFYwMxSdZraw5PkqwtqY9zLDF", (PublicKey)owner);
        }

        [TestMethod]
        public void DecodeInstructionsFromTransactionMetaTest()
        {
            string responseData = File.ReadAllText("Resources/TestDecodeInstructionFromBlockTransactionMetaInfo.json");
            TransactionMetaInfo txMeta = JsonSerializer.Deserialize<TransactionMetaInfo>(responseData,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                });

            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(txMeta);

            Assert.AreEqual(3, decodedInstructions.Count);

            Assert.AreEqual("Create Associated Token Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Associated Token Account Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("ATokenGPvbdGVxr1b2hvZbsiqW5xWH25efTNsLJA8knL",
                decodedInstructions[0].PublicKey);
            Assert.AreEqual(4, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Payer", out object payer));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Associated Token Account Address",
                out object associatedAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)payer);
            Assert.AreEqual("BrvPSQpe6rYdvsS4idWPSKdUzyF8v3ZySVYYTuyCJnH5", (PublicKey)associatedAccount);
            Assert.AreEqual("65EoWs57dkMEWbK4TJkPDM76rnbumq7r3fiZJnxggj2G", (PublicKey)owner);
            Assert.AreEqual("4NtWFCwJDebDw16pEPh9JJo9XkuufK1tvY8A2MmkrsRP", (PublicKey)mint);

            Assert.AreEqual("Transfer", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA",
                decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out owner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Amount", out object amount));
            Assert.AreEqual("DEy4VaFFqTn6MweESovsbA5mUDMD2a99qnT8YMKSrCF3", (PublicKey)source);
            Assert.AreEqual("BrvPSQpe6rYdvsS4idWPSKdUzyF8v3ZySVYYTuyCJnH5", (PublicKey)destination);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual(25000UL, (ulong)amount);

            Assert.AreEqual("New Memo", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Memo Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("Memo1UhkJRfHyvLMcVucJwxXeuD728EqVDDwQDxFMNo",
                decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer", out object signer));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Memo", out object memo));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)signer);
            Assert.AreEqual("Hello from Sol.Net", (string)memo);
        }

        [TestMethod]
        public void DecodeInstructionsFromTransactionUnknownInstructionTest()
        {
            string responseData = File.ReadAllText("Resources/TestDecodeFromTransactionUnknownInstruction.json");
            TransactionMetaSlotInfo txMeta = JsonSerializer.Deserialize<TransactionMetaSlotInfo>(responseData,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                });

            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(txMeta);

            Assert.AreEqual(4, decodedInstructions.Count);

            Assert.AreEqual("Unknown", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[2].ProgramName);
            Assert.AreEqual("auctxRXPeJoc4817jDhf4HbjnhEcr1cCXenosMhK5R8",
                decodedInstructions[2].PublicKey);
            Assert.AreEqual(1, decodedInstructions[2].InnerInstructions.Count);
        }


        [TestMethod]
        public void DecodeInstructionsFromTransactionUnknownInnerInstructionTest()
        {
            string responseData = File.ReadAllText("Resources/TestDecodeFromTransactionUnknownInnerInstruction.json");
            TransactionMetaSlotInfo txMeta = JsonSerializer.Deserialize<TransactionMetaSlotInfo>(responseData,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
                });

            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(txMeta);
            Assert.AreEqual(2, decodedInstructions.Count);

            Assert.AreEqual("Unknown", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[0].ProgramName);
            Assert.AreEqual("675kPX9MHTjS2zt1qfr1NYHuzeLXfQM9H24wFSUt1Mp8",
                decodedInstructions[0].PublicKey);
            Assert.AreEqual(3, decodedInstructions[0].InnerInstructions.Count);
            Assert.AreEqual("Unknown", decodedInstructions[0].InnerInstructions[0].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[0].InnerInstructions[0].ProgramName);
            Assert.AreEqual("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin",
                decodedInstructions[0].InnerInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions[0].InnerInstructions.Count);

            Assert.AreEqual("Unknown", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[1].ProgramName);
            Assert.AreEqual("675kPX9MHTjS2zt1qfr1NYHuzeLXfQM9H24wFSUt1Mp8",
                decodedInstructions[1].PublicKey);
            Assert.AreEqual(3, decodedInstructions[1].InnerInstructions.Count);
            Assert.AreEqual("Unknown", decodedInstructions[1].InnerInstructions[0].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[1].InnerInstructions[0].ProgramName);
            Assert.AreEqual("9xQeWvG816bUx9EPjHmaT23yvVM2ZWbrrpZb9PusVFin",
                decodedInstructions[1].InnerInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions[0].InnerInstructions.Count);
        }

        [TestMethod]
        public void DecodeUnknownInstructionFromMessageTest()
        {
            Message msg = Message.Deserialize(UnknownInstructionMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, decodedInstructions.Count);

            Assert.AreEqual("Unknown", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Unknown", decodedInstructions[2].ProgramName);
            Assert.AreEqual("HgQBwfas29FTc2hFw2KfdtrhChYVfk5LmMraSHUTTh9L", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
        }

        [TestMethod]
        public void DecodeCreateNameRegistryDecodeTest()
        {
            Message msg = Message.Deserialize(CreateNameRecordMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);

            // Create name registry instruction
            Assert.AreEqual("Create Name Record", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Name Service Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Payer", out object payer));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Name Account", out object nameAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Name Owner", out object nameOwner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Parent Name", out object parentName));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Lamports", out object lamports));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Hashed Name", out object hashedName));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Hashed Name Length", out object hashedNameLength));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)payer);
            Assert.AreEqual("FZbmrZNps5ChFibRphoxdHNE2M2yrforXprAkDiCMA5q", (PublicKey)nameAccount);
            Assert.AreEqual("GzgX4Lv8wQp5o8ovtjT8w6ukQT98HB2a7vg7s1PXqydF", (PublicKey)nameOwner);
            Assert.AreEqual("11111111111111111111111111111111", (PublicKey)parentName);
            Assert.AreEqual(1684320UL, (ulong)lamports);
            Assert.AreEqual(1096UL, (uint)space);
            Assert.AreEqual(32U, (uint)hashedNameLength);
            Assert.AreEqual("pzT91B4byCH5j/ED1h+mUHphJItZaJ/JIbddoj769Sw=", Convert.ToBase64String((byte[])hashedName));
        }

        [TestMethod]
        public void DecodeCreateNameRegistryFullTest()
        {
            Message msg = Message.Deserialize(CreateNameRecordMessageWithOptionals);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);
            Assert.AreEqual(3, decodedInstructions.Count);

            // Create name registry instruction
            Assert.AreEqual("Create Name Record", decodedInstructions[0].InstructionName);
            Assert.AreEqual("Name Service Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Payer", out object payer));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Name Account", out object nameAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Name Owner", out object nameOwner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Name Class", out object nameClass));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Parent Name", out object parentName));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Parent Name Owner", out object parentNameOwner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Lamports", out object lamports));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Hashed Name", out object hashedName));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Hashed Name Length", out object hashedNameLength));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)payer);
            Assert.AreEqual("H3eeTLmTHm3NN5PGMptP1n77H6ax2Bg86eCrDnP8p5M7", (PublicKey)nameAccount);
            Assert.AreEqual("GzgX4Lv8wQp5o8ovtjT8w6ukQT98HB2a7vg7s1PXqydF", (PublicKey)nameOwner);
            Assert.AreEqual("8ZhEweTBhjTVzuRyoJteCqNU7AiHdpYTfreD1y9FvoFu", (PublicKey)nameClass);
            Assert.AreEqual("CtMQ63EE8dNn3L8fmTfHLXgyBiZUFEzC243RCTAX6fgK", (PublicKey)parentName);
            Assert.AreEqual("6kckCE9VHfncVLEsVFFsdc3mmffyECScACPS8NnYxsf5", (PublicKey)parentNameOwner);
            Assert.AreEqual(1684320UL, (ulong)lamports);
            Assert.AreEqual(1096UL, (uint)space);
            Assert.AreEqual(32U, (uint)hashedNameLength);
            Assert.AreEqual("pzT91B4byCH5j/ED1h+mUHphJItZaJ/JIbddoj769Sw=", Convert.ToBase64String((byte[])hashedName));
        }

        [TestMethod]
        public void DecodeUpdateNameRegistryFullTest()
        {
            Message msg = Message.Deserialize(UpdateNameRegistryMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);

            // update name registry instruction
            Assert.AreEqual("Update Name Record", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Name Service Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Account", out object nameAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Class", out object nameClass));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Offset", out object offset));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Data", out object data));
            Assert.AreEqual("DBJo83875zELNsWd8zicSTFhAzyFAVgKmcRvWv8tndvX", (PublicKey)nameAccount);
            Assert.AreEqual("8ZhEweTBhjTVzuRyoJteCqNU7AiHdpYTfreD1y9FvoFu", (PublicKey)nameClass);
            Assert.AreEqual(125U, (uint)offset);
            CollectionAssert.AreEqual(new byte[] { 0, 0, 1, 1 }, (byte[])data);
        }

        [TestMethod]
        public void DecodeTransferNameRegistryFullTest()
        {
            Message msg = Message.Deserialize(TransferNameRegistryMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);

            // update name registry instruction
            Assert.AreEqual("Transfer Name Record", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Name Service Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Account", out object nameAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Owner", out object nameOwner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Class", out object nameClass));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("New Owner", out object newOwner));
            Assert.AreEqual("DBJo83875zELNsWd8zicSTFhAzyFAVgKmcRvWv8tndvX", (PublicKey)nameAccount);
            Assert.AreEqual("8ZhEweTBhjTVzuRyoJteCqNU7AiHdpYTfreD1y9FvoFu", (PublicKey)nameClass);
            Assert.AreEqual("GzgX4Lv8wQp5o8ovtjT8w6ukQT98HB2a7vg7s1PXqydF", (PublicKey)nameOwner);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)newOwner);
        }

        [TestMethod]
        public void DecodeDeleteNameRegistryFullTest()
        {
            Message msg = Message.Deserialize(DeleteNameRegistryMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);

            // update name registry instruction
            Assert.AreEqual("Delete Name Record", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Name Service Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("namesLPneVptA9Z5rqUDD9tMTWEJwofgaYwp8cawRkX", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Account", out object nameAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Name Owner", out object nameOwner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Refund Account", out object refundAccount));
            Assert.AreEqual("DBJo83875zELNsWd8zicSTFhAzyFAVgKmcRvWv8tndvX", (PublicKey)nameAccount);
            Assert.AreEqual("GzgX4Lv8wQp5o8ovtjT8w6ukQT98HB2a7vg7s1PXqydF", (PublicKey)nameOwner);
            Assert.AreEqual("8ZhEweTBhjTVzuRyoJteCqNU7AiHdpYTfreD1y9FvoFu", (PublicKey)refundAccount);
        }

        [TestMethod]
        public void DecodeSharedMemoryProgramTest()
        {
            Message msg = Message.Deserialize(SharedMemoryWriteMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, decodedInstructions.Count);
            Assert.AreEqual("Write", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Shared Memory Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("shmem4EWT2sPdVGvTZCzXXRAURL9G5vpPxNwSeKhHUL", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Offset", out object offset));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Data", out object data));
            Assert.AreEqual(35UL, (ulong)offset);
            CollectionAssert.AreEqual(new byte[] { 1, 0, 15, 25, 77, }, (byte[])data);
        }

        [TestMethod]
        public void DecodeDurableNonceMessageTest()
        {
            Message msg = Message.Deserialize(DurableNonceMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Advance Nonce Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Nonce Account", out object nonceAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Authority", out object authority));
            Assert.AreEqual("G5EWCBwDM5GzVNwrG9LbgpTdQBD9PEAaey82ttuJJ7Qo", (PublicKey)nonceAccount);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)authority);
        }

        [TestMethod]
        public void DecodeCreateAccountWithSeedTest()
        {
            Message msg = Message.Deserialize(CreateWithSeedTransferCheckedMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(3, decodedInstructions.Count);
            Assert.AreEqual("Create Account With Seed", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("From Account", out object fromAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("To Account", out object toAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Base Account", out object baseAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Seed", out object seed));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner", out object ownerAccount));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)fromAccount);
            Assert.AreEqual("4sW9XdttQsm1QrfQoRW95jMX4Q5jWYjKkSPEAmkndDUY", (PublicKey)toAccount);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)baseAccount);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", (PublicKey)ownerAccount);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", (PublicKey)ownerAccount);
            Assert.AreEqual(2039280UL, (ulong)amount);
            Assert.AreEqual(165UL, (ulong)space);
            Assert.AreEqual("Some Seed", (string)seed);


            Assert.AreEqual("Transfer Checked", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[2].PublicKey);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Source", out object source));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Mint", out object mint));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Authority", out ownerAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Amount", out amount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Decimals", out object decimals));
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)source);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("4sW9XdttQsm1QrfQoRW95jMX4Q5jWYjKkSPEAmkndDUY", (PublicKey)destination);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)ownerAccount);
            Assert.AreEqual(10000UL, (ulong)amount);
            Assert.AreEqual(10, (byte)decimals);
        }


        [TestMethod]
        public void DecodeAllocateAndTransferWithSeedTest()
        {
            Message msg = Message.Deserialize(AllocateAndTransferWithSeedMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(6, decodedInstructions.Count);
            Assert.AreEqual("Allocate With Seed", decodedInstructions[1].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Base Account", out object baseAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Owner", out object ownerAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Seed", out object seed));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("EME9GxLahsC1mjopepKMJg9RtbUu37aeLaQyHVdEd7vZ", (PublicKey)account);
            Assert.AreEqual("Gg12mmahG97PDACxKiBta7ch2kkqDkXUzjn5oAcbPZct", (PublicKey)baseAccount);
            Assert.AreEqual("J6WZY5nuYGJmfFtBGZaXgwZSRVuLWxNR6gd4d3XTHqTk", (PublicKey)ownerAccount);
            Assert.AreEqual(165UL, (ulong)space);
            Assert.AreEqual("Some Seed", (string)seed);

            Assert.AreEqual("Transfer With Seed", decodedInstructions[2].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("From Account", out object fromAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("From Base Account", out object fromBaseAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("To Account", out object toAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("From Owner", out object fromOwnerAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Seed", out seed));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Amount", out object amount));
            Assert.AreEqual("Gg12mmahG97PDACxKiBta7ch2kkqDkXUzjn5oAcbPZct", (PublicKey)fromAccount);
            Assert.AreEqual("EME9GxLahsC1mjopepKMJg9RtbUu37aeLaQyHVdEd7vZ", (PublicKey)fromBaseAccount);
            Assert.AreEqual("EME9GxLahsC1mjopepKMJg9RtbUu37aeLaQyHVdEd7vZ", (PublicKey)toAccount);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)fromOwnerAccount);
            Assert.AreEqual(25000UL, (ulong)amount);
            Assert.AreEqual("Some Seed", (string)seed);
        }

        [TestMethod]
        public void DecodeAssignWithSeedAndWithdrawNonceTest()
        {
            Message msg = Message.Deserialize(AssignWithSeedAndWithdrawNonceMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(6, decodedInstructions.Count);
            Assert.AreEqual("Assign With Seed", decodedInstructions[1].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Base Account", out object baseAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Owner", out object ownerAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Seed", out object seed));
            Assert.AreEqual("EME9GxLahsC1mjopepKMJg9RtbUu37aeLaQyHVdEd7vZ", (PublicKey)account);
            Assert.AreEqual("Gg12mmahG97PDACxKiBta7ch2kkqDkXUzjn5oAcbPZct", (PublicKey)baseAccount);
            Assert.AreEqual("J6WZY5nuYGJmfFtBGZaXgwZSRVuLWxNR6gd4d3XTHqTk", (PublicKey)ownerAccount);
            Assert.AreEqual("Some Seed", (string)seed);

            Assert.AreEqual("Withdraw Nonce Account", decodedInstructions[2].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Nonce Account", out object nonceAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("To Account", out object toAccount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Authority", out object authority));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Amount", out object amount));
            Assert.AreEqual("Gg12mmahG97PDACxKiBta7ch2kkqDkXUzjn5oAcbPZct", (PublicKey)nonceAccount);
            Assert.AreEqual("EME9GxLahsC1mjopepKMJg9RtbUu37aeLaQyHVdEd7vZ", (PublicKey)toAccount);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)authority);
            Assert.AreEqual(25000UL, (ulong)amount);
        }

        [TestMethod]
        public void DecodeCreateNonceAccountTest()
        {
            Message msg = Message.Deserialize(CreateNonceAccountMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("Initialize Nonce Account", decodedInstructions[1].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Nonce Account", out object nonceAccount));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Authority", out object authority));
            Assert.AreEqual("G5EWCBwDM5GzVNwrG9LbgpTdQBD9PEAaey82ttuJJ7Qo", (PublicKey)nonceAccount);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)authority);
        }

        [TestMethod]
        public void DecodeAuthorizeNonceAccountTest()
        {
            Message msg = Message.Deserialize(AuthorizeNonceAccountMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("Authorize Nonce Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Nonce Account", out object nonceAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Current Authority", out object currentAuthority));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Authority", out object newAuthority));
            Assert.AreEqual("G5EWCBwDM5GzVNwrG9LbgpTdQBD9PEAaey82ttuJJ7Qo", (PublicKey)nonceAccount);
            Assert.AreEqual("3rw6fodqaBQHQZgMuFzbkfz7KNd1H999PphPMJwbqV53", (PublicKey)newAuthority);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)currentAuthority);
        }
    }
}