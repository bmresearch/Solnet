using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Rpc.Builders;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using Solnet.Wallet.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Rpc.Test
{
    [TestClass]
    public class MessageTest
    {
        private static byte[] MessageBytes =
        {
            2, 0, 4, 6, 103, 132, 83, 145, 168, 194, 85, 123, 82, 13, 216, 210, 8, 202, 191, 237, 245, 126, 242,
            121, 138, 175, 133, 11, 234, 99, 249, 185, 73, 124, 75, 186, 152, 4, 15, 191, 192, 69, 38, 242, 209, 25,
            50, 6, 106, 251, 40, 228, 145, 75, 129, 20, 113, 52, 100, 202, 150, 100, 146, 135, 243, 11, 171, 9, 139,
            31, 75, 78, 123, 162, 191, 215, 73, 252, 141, 86, 38, 131, 208, 130, 205, 241, 73, 237, 15, 207, 180,
            165, 130, 89, 152, 200, 252, 194, 65, 137, 6, 167, 213, 23, 25, 44, 92, 81, 33, 140, 201, 76, 61, 74,
            241, 127, 88, 218, 238, 8, 155, 161, 253, 68, 227, 219, 217, 138, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 161,
            147, 217, 203, 225, 70, 206, 235, 121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126,
            255, 0, 169, 83, 184, 173, 154, 195, 40, 140, 119, 87, 191, 63, 56, 94, 10, 92, 101, 22, 75, 177, 41,
            209, 154, 164, 40, 179, 236, 193, 222, 193, 120, 75, 193, 2, 4, 2, 0, 1, 52, 0, 0, 0, 0, 240, 29, 31, 0,
            0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 161, 147, 217, 203, 225, 70, 206, 235,
            121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169, 5, 4, 1, 2, 0, 3, 1,
            1
        };

        private const string Base64Message =
            "AgAEBmeEU5GowlV7Ug3Y0gjKv+31fvJ5iq+FC+pj+blJfEu615Bs5Vo6mnXZXvh35ULmThtyhwH8xzDk8CgGqB1ISymLH0tOe6K/10n8jVYmg9CCzfFJ7Q/PtKWCWZjI/MJBiQan1RcZLFxRIYzJTD1K8X9Y2u4Im6H9ROPb2YoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqeIfQzb6ERv8S2AqP3kpqFe1rhOi8a8q+HoB5Z/4WUfiAgQCAAE0AAAAAPAdHwAAAAAApQAAAAAAAAAG3fbh12Whk9nL4UbO63msHLSF7V9bN5E6jPWFfv8AqQUEAQIAAwEB";

        [TestMethod]
        public void MessageDeserializeTest()
        {
            Message msg = Message.Deserialize(Base64Message);

            Assert.IsNotNull(msg);
            Assert.AreEqual(2, msg.Header.RequiredSignatures);
            Assert.AreEqual(0, msg.Header.ReadOnlySignedAccounts);
            Assert.AreEqual(4, msg.Header.ReadOnlyUnsignedAccounts);
            Assert.AreEqual("GDgnjNiNGnw9nA3diFYKeizi8LpBzFMjDaBSU5hoqEUH", msg.RecentBlockhash);
            Assert.AreEqual(6, msg.AccountKeys.Count);
            Assert.AreEqual("7y62LXLwANaN9g3KJPxQFYwMxSdZraw5PkqwtqY9zLDF", msg.AccountKeys[0].Key);
            Assert.AreEqual("FWUPMzrLbAEuH83cf1QphoFdyUdhenDF5oHftwd9Vjyr", msg.AccountKeys[1].Key);
            Assert.AreEqual("AN5M7KvEFiZFxgEUWFdZUdR5i4b96HjXawADpqjxjXCL", msg.AccountKeys[2].Key);
            Assert.AreEqual("SysvarRent111111111111111111111111111111111", msg.AccountKeys[3].Key);
            Assert.AreEqual("11111111111111111111111111111111", msg.AccountKeys[4].Key);
            Assert.AreEqual("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA", msg.AccountKeys[5].Key);

            Assert.AreEqual(2, msg.Instructions.Count);
            Assert.AreEqual(4, msg.Instructions[0].ProgramIdIndex);
            CollectionAssert.AreEqual(new byte[] { 0, 1 }, msg.Instructions[0].KeyIndices);
            CollectionAssert.AreEqual(new byte[] { 2 }, msg.Instructions[0].KeyIndicesCount);
            CollectionAssert.AreEqual(new byte[] { 52 }, msg.Instructions[0].DataLength);
            CollectionAssert.AreEqual(new byte[]{0, 0, 0, 0, 240, 29, 31, 0,
                0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 161, 147, 217, 203, 225, 70, 206, 235,
                121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169}, msg.Instructions[0].Data);

            Assert.AreEqual(5, msg.Instructions[1].ProgramIdIndex);
            CollectionAssert.AreEqual(new byte[] { 1, 2, 0, 3 }, msg.Instructions[1].KeyIndices);
            CollectionAssert.AreEqual(new byte[] { 4 }, msg.Instructions[1].KeyIndicesCount);
            CollectionAssert.AreEqual(new byte[] { 1 }, msg.Instructions[1].DataLength);
            CollectionAssert.AreEqual(new byte[] { 1 }, msg.Instructions[1].Data);
        }

        [TestMethod]
        public void MessageSerializeTest()
        {
            Message msg = new()
            {
                Header = new MessageHeader
                {
                    RequiredSignatures = 2,
                    ReadOnlySignedAccounts = 0,
                    ReadOnlyUnsignedAccounts = 4
                },
                RecentBlockhash = "6dpApBv7syEswXqBMkyHqETN3MGY5x4ZW2cnLzRSSLJ4",
                AccountKeys = new List<PublicKey>
                {
                    new ("7y62LXLwANaN9g3KJPxQFYwMxSdZraw5PkqwtqY9zLDF"),
                    new ("BEQZbsAdm5tRgtezK65oFgQwYanujpxx96jYo7Nkp592"),
                    new ("AN5M7KvEFiZFxgEUWFdZUdR5i4b96HjXawADpqjxjXCL"),
                    new ("SysvarRent111111111111111111111111111111111"),
                    new ("11111111111111111111111111111111"),
                    new ("TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA"),
                },
                Instructions = new List<CompiledInstruction>
                {
                    new ()
                    {
                        KeyIndices = new byte[]{0, 1},
                        KeyIndicesCount = new byte[]{2},
                        DataLength = new byte[]{52},
                        Data = new byte[]{0, 0, 0, 0, 240, 29, 31, 0,
                            0, 0, 0, 0, 165, 0, 0, 0, 0, 0, 0, 0, 6, 221, 246, 225, 215, 101, 161, 147, 217, 203, 225, 70, 206, 235,
                            121, 172, 28, 180, 133, 237, 95, 91, 55, 145, 58, 140, 245, 133, 126, 255, 0, 169},
                        ProgramIdIndex = 4
                    },
                    new ()
                    {
                        KeyIndices = new byte[]{ 1, 2, 0, 3},
                        KeyIndicesCount = new byte[]{4},
                        DataLength = new byte[]{1},
                        Data = new byte[]{1},
                        ProgramIdIndex = 5
                    }
                }
            };

            CollectionAssert.AreEqual(msg.Serialize(), MessageBytes);
        }
    }
}