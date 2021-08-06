using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Core.Http;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        private const string InitializeMultisigMessage =
            "AwAJDEdpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyeLALNX+Hq5QvYpjBUrxcE6c1OPFtuOsWTs" +
            "RwZ22JTNv0sF4mdbv4FGc/JcD4qM+DJXE0k+DhmNmPu8MItrFyfgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAAAAAAAABqfVFxksXFEhjMlMPUrxf1ja7gibof1E49vZigAAAAC9PD4jUE81HRWVKjhuaeGhBDrUiRU" +
            "sQ8PRa6Gkh7BcAzbV0glem2ocQYDPKtsvb2P6eY+diK2RlCQbryCDiW9ENqhqvd4wlbvt2WLwsRs1GuOPhm" +
            "Rt728O9WHpObgVQ60+Im+a09G04MQPhepwoQn2VGuSmOoDsZvfRJ8im8hThYp3QXZN2eL1ihOJMfLOtOE0d" +
            "btnaKq58W0jnl+pjmXBBt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX7/AKkFSlNQ+F3IgtYUpVZyeIop" +
            "bd8eq6vQpgZ4iEky9O72oNRsOMzYJJil8tqxLyZCv3xaGw9O1hPoqUsFwShXE+aABQMCAAE0AAAAAJBLMwA" +
            "AAAAAYwEAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQoHAQQFBgcICQICAwMCAAI0AA" +
            "AAAGBNFgAAAAAAUgAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQoCAgRDAAp4sAs1f" +
            "4erlC9imMFSvFwTpzU48W246xZOxHBnbYlM2wBT7yGUtArURga4Avg+yhMwOEM69UaXYBPa+5CFN2YhDQsB" +
            "ABJIZWxsbyBmcm9tIFNvbC5OZXQ=";

        private const string MintToMultisigMessage =
            "BQMFC0dpq5cgS6g/sMruF/eGjx4HTlIVgaDYnZQ3napltxeyDpwtYu322rjxMK+ED8DutHhxOkdgN0Rl6/B7o" +
            "VsMMG69PD4jUE81HRWVKjhuaeGhBDrUiRUsQ8PRa6Gkh7BcAzbV0glem2ocQYDPKtsvb2P6eY+diK2RlCQbry" +
            "CDiW9EPiJvmtPRtODED4XqcKEJ9lRrkpjqA7Gb30SfIpvIU4X0sF4mdbv4FGc/JcD4qM+DJXE0k+DhmNmPu8" +
            "MItrFyfgAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAABqfVFxksXFEhjMlMPUrxf1ja7gibof1E49" +
            "vZigAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqXiwCzV/h6uUL2KYwVK8XBOnNTjxbbjrFk" +
            "7EcGdtiUzbBUpTUPhdyILWFKVWcniKKW3fHqur0KYGeIhJMvTu9qD2GZ+Dnx/yuoM4nlAAN0csYxYXMvDV/e" +
            "u6teeG3c6leQQGAgABNAAAAADwHR8AAAAAAKUAAAAAAAAABt324ddloZPZy+FGzut5rBy0he1fWzeROoz1hX" +
            "7/AKkIBAEFAAcBAQgGBQEJAgMECQeoYQAAAAAAAAoBABJIZWxsbyBmcm9tIFNvbC5OZXQ=";
        
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
        public void TestDecodeInstructionsFromMessage()
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
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Owner", out owner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual("FWUPMzrLbAEuH83cf1QphoFdyUdhenDF5oHftwd9Vjyr", (PublicKey)account);
            Assert.AreEqual("AN5M7KvEFiZFxgEUWFdZUdR5i4b96HjXawADpqjxjXCL", (PublicKey)mint);
            Assert.AreEqual("7y62LXLwANaN9g3KJPxQFYwMxSdZraw5PkqwtqY9zLDF", (PublicKey)owner);
        }

        [TestMethod]
        public void TestDecodeInstructionsFromTransactionMeta()
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
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Owner", out owner));
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
        public void CreateNameRegistryDecodeTest()
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
        public void CreateNameRegistryFullTest()
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
        public void UpdateNameRegistryFullTest()
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
            CollectionAssert.AreEqual(new byte[] {0,0,1,1}, (byte[]) data);
        }
        
        [TestMethod]
        public void TransferNameRegistryFullTest()
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
        public void DeleteNameRegistryFullTest()
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
        public void InitializeMultisigDecodeTest()
        {
            Message msg = Message.Deserialize(InitializeMultisigMessage);

            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(5, decodedInstructions.Count);

            // Create Account instruction
            Assert.AreEqual("Create Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner Account", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Account", out object newAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)newAccount);
            Assert.AreEqual(3361680UL, (ulong)amount);
            Assert.AreEqual(355UL, (ulong)space);

            // initialize multisig instruction
            Assert.AreEqual("Initialize Multisig", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Required Signers", out object numReqSigners));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 3", out object signer3));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 4", out object signer4));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Signer 5", out object signer5));
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)account);
            Assert.AreEqual(3, (byte)numReqSigners);
            Assert.AreEqual("DjhLN52wpL6aw9k65MHb3jwxQ7fZ7gfMUGK3gHMBQPWa", (PublicKey)signer1);
            Assert.AreEqual("4h47wFJ7dheVfJJrEcQfx5HvsP3PsfxEqaN38E6pSfhd", (PublicKey)signer2);
            Assert.AreEqual("4gMxwYxoxbSekFNEUtUFfWECF5cp2FRGughfMx22ivwe", (PublicKey)signer3);
            Assert.AreEqual("5BYjVTAYDrRQpMCP4zML3X2v6Jde1sHx3a1bd6DRskVJ", (PublicKey)signer4);
            Assert.AreEqual("AKWjVdBUvekPc2bGf6gKAbQNRSfiXVZ3qFVnP6W8p1W8", (PublicKey)signer5);

            // initialize mint instruction
            Assert.AreEqual("Initialize Mint", decodedInstructions[3].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[3].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[3].PublicKey);
            Assert.AreEqual(0, decodedInstructions[3].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Account", out account));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Decimals", out object decimals));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Mint Authority", out object mintAuthority));
            Assert.IsTrue(decodedInstructions[3].Values.TryGetValue("Freeze Authority", out object freezeAuthority));
            Assert.IsTrue(decodedInstructions[3].Values
                .TryGetValue("Freeze Authority Option", out object freezeAuthorityOpt));
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)account);
            Assert.AreEqual(10, (byte)decimals);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)mintAuthority);
            Assert.AreEqual(0, (byte)freezeAuthorityOpt);
            Assert.AreEqual("6eeL1Wb4ufcnxjTtvEStVGHPHeAWexLAFcJ6Kq9pUsXJ", (PublicKey)freezeAuthority);
        }

        [TestMethod]
        public void MintToMultisigDecodeTest()
        {
            Message msg = Message.Deserialize(MintToMultisigMessage);

            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(4, decodedInstructions.Count);
            
            // Create Account instruction
            Assert.AreEqual("Create Account", decodedInstructions[0].InstructionName);
            Assert.AreEqual("System Program", decodedInstructions[0].ProgramName);
            Assert.AreEqual("11111111111111111111111111111111", decodedInstructions[0].PublicKey);
            Assert.AreEqual(0, decodedInstructions[0].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Owner Account", out object owner));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("New Account", out object newAccount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Amount", out object amount));
            Assert.IsTrue(decodedInstructions[0].Values.TryGetValue("Space", out object space));
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)newAccount);
            Assert.AreEqual(2039280UL, (ulong)amount);
            Assert.AreEqual(165UL, (ulong)space);
            
            // initialize account instruction
            Assert.AreEqual("Initialize Account", decodedInstructions[1].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[1].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[1].PublicKey);
            Assert.AreEqual(0, decodedInstructions[1].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Account", out object account));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Owner", out owner));
            Assert.IsTrue(decodedInstructions[1].Values.TryGetValue("Mint", out object mint));
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)account);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("5omQJtDUHA3gMFdHEQg1zZSvcBUVzey5WaKWYRmqF1Vj", (PublicKey)owner);
            
            // mint to multisig instruction
            Assert.AreEqual("Mint To", decodedInstructions[2].InstructionName);
            Assert.AreEqual("Token Program", decodedInstructions[2].ProgramName);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", decodedInstructions[2].PublicKey);
            Assert.AreEqual(0, decodedInstructions[2].InnerInstructions.Count);
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Mint Authority", out object mintAuthority));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Destination", out object destination));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Amount", out amount));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Mint", out mint));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 1", out object signer1));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 2", out object signer2));
            Assert.IsTrue(decodedInstructions[2].Values.TryGetValue("Signer 3", out object signer3));
            Assert.AreEqual(25000UL, (ulong)amount);
            Assert.AreEqual("DjhLN52wpL6aw9k65MHb3jwxQ7fZ7gfMUGK3gHMBQPWa", (PublicKey)signer1);
            Assert.AreEqual("4h47wFJ7dheVfJJrEcQfx5HvsP3PsfxEqaN38E6pSfhd", (PublicKey)signer2);
            Assert.AreEqual("5BYjVTAYDrRQpMCP4zML3X2v6Jde1sHx3a1bd6DRskVJ", (PublicKey)signer3);
            Assert.AreEqual("z2qF2eWM89sQrXP2ygrLkYkhc58182KqPVRETjv8Dch", (PublicKey)destination);
            Assert.AreEqual("HUATcRqk8qaNHTfRjBePt9mUZ16dDN1cbpWQDk7QFUGm", (PublicKey)mint);
            Assert.AreEqual("987cq6uofpTKzTyQywsyqNNyAKHAkJkBvY6ggqPnS8gJ", (PublicKey)mintAuthority);
        }
    }
}