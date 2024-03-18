using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.ComputeBudget;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class ComputeBudgetProgramTest
    {
        private static readonly byte[] ComputeBudgetProgramIdBytes =
        {
            3, 6, 70, 111, 229, 33, 23, 50, 255, 236, 173, 186, 114, 195, 155, 231, 
            188, 140, 229, 187, 197, 247, 18, 107, 44, 67, 155, 58, 64, 0, 0, 0
        };

        private static readonly byte[] RequestHeapFrameInstructionBytes =
        {
            1, 0, 128, 0, 0
        };

        private static readonly byte[] SetComputeUnitLimitInstructionBytes =
        {
            2, 64, 13, 3, 0
        };

        private static readonly byte[] SetComputeUnitPriceInstructionBytes =
        {
            3, 160, 134, 1, 0, 0, 0, 0, 0
        };

        private static readonly byte[] SetLoadedAccountsDataSizeLimitInstructionBytes =
        {
            4, 48, 87, 5, 0
        };

        [TestMethod]
        public void TestComputeBudgetProgramRequestHeapFrame()
        {
            var txInstruction = ComputeBudgetProgram.RequestHeapFrame(32 * 1024);

            Assert.AreEqual(0, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(RequestHeapFrameInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(ComputeBudgetProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestComputeBudgetProgramSetComputeUnitLimit()
        {
            var txInstruction = ComputeBudgetProgram.SetComputeUnitLimit(200000);

            Assert.AreEqual(0, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetComputeUnitLimitInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(ComputeBudgetProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestComputeBudgetProgramSetComputeUnitPrice()
        {
            var txInstruction = ComputeBudgetProgram.SetComputeUnitPrice(100000);

            Assert.AreEqual(0, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetComputeUnitPriceInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(ComputeBudgetProgramIdBytes, txInstruction.ProgramId);
        }

        [TestMethod]
        public void TestComputeBudgetProgramSetLoadedAccountsDataSizeLimit()
        {
            var txInstruction = ComputeBudgetProgram.SetLoadedAccountsDataSizeLimit(350000);

            Assert.AreEqual(0, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(SetLoadedAccountsDataSizeLimitInstructionBytes, txInstruction.Data);
            CollectionAssert.AreEqual(ComputeBudgetProgramIdBytes, txInstruction.ProgramId);
        }
    }
}