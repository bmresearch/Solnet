using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet;
using System.Linq;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class AccountCompressionProgramTest
    {
        private static readonly PublicKey MerkleTree = new("11111111111111111111111111111111");
        private static readonly PublicKey Authority = new("22222222222222222222222222222222");
        private static readonly PublicKey Recipient = new("33333333333333333333333333333333");
        private static readonly PublicKey NewAuthority = new("44444444444444444444444444444444");

        [TestMethod]
        public void Append_CreatesCorrectInstruction()
        {
            var leaf = Enumerable.Range(0, 32).Select(i => (byte)i).ToArray();
            var instr = AccountCompressionProgram.Append(MerkleTree, Authority, leaf);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(2, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable && instr.Keys[0].IsSigner); // Authority
            Assert.IsTrue(instr.Keys[1].IsWritable && !instr.Keys[1].IsSigner); // MerkleTree
            Assert.AreEqual(36, instr.Data.Length);
        }

        [TestMethod]
        public void CloseEmptyTree_CreatesCorrectInstruction()
        {
            var instr = AccountCompressionProgram.CloseEmptyTree(MerkleTree, Authority, Recipient);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(3, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.IsFalse(instr.Keys[2].IsWritable); // Recipient
            Assert.AreEqual(4, instr.Data.Length);
        }

        [TestMethod]
        public void InitEmptyMerkleTree_CreatesCorrectInstruction()
        {
            byte maxDepth = 10, maxBufferSize = 20;
            var instr = AccountCompressionProgram.InitEmptyMerkleTree(MerkleTree, Authority, maxDepth, maxBufferSize);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(2, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.AreEqual(6, instr.Data.Length);
        }

        [TestMethod]
        public void ReplaceLeaf_CreatesCorrectInstruction()
        {
            var newLeaf = Enumerable.Repeat((byte)1, 32).ToArray();
            var prevLeaf = Enumerable.Repeat((byte)2, 32).ToArray();
            var root = Enumerable.Repeat((byte)3, 32).ToArray();
            uint index = 42;

            var instr = AccountCompressionProgram.ReplaceLeaf(MerkleTree, Authority, newLeaf, prevLeaf, root, index);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(2, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.AreEqual(104, instr.Data.Length);
        }

        [TestMethod]
        public void InsertOrAppend_CreatesCorrectInstruction()
        {
            var leaf = Enumerable.Repeat((byte)1, 32).ToArray();
            var root = Enumerable.Repeat((byte)2, 32).ToArray();
            uint index = 99;

            var instr = AccountCompressionProgram.InsertOrAppend(MerkleTree, Authority, leaf, root, index);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(2, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.AreEqual(72, instr.Data.Length);
        }

        [TestMethod]
        public void TransferAuthority_CreatesCorrectInstruction()
        {
            var instr = AccountCompressionProgram.TransferAuthority(MerkleTree, Authority, NewAuthority);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(3, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.IsFalse(instr.Keys[2].IsWritable); // NewAuthority
            Assert.AreEqual(36, instr.Data.Length);
        }

        [TestMethod]
        public void VerifyLeaf_CreatesCorrectInstruction()
        {
            var root = Enumerable.Repeat((byte)1, 32).ToArray();
            var leaf = Enumerable.Repeat((byte)2, 32).ToArray();
            uint index = 123;

            var instr = AccountCompressionProgram.VerifyLeaf(MerkleTree, Authority, root, leaf, index);

            CollectionAssert.AreEqual(AccountCompressionProgram.ProgramIdKey.KeyBytes, instr.ProgramId);
            Assert.AreEqual(2, instr.Keys.Count);
            Assert.IsTrue(instr.Keys[0].IsWritable); // MerkleTree
            Assert.IsTrue(instr.Keys[1].IsWritable && instr.Keys[1].IsSigner); // Authority
            Assert.AreEqual(72, instr.Data.Length);
        }
    }
}
