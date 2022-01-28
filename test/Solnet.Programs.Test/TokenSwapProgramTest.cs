using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Programs.TokenSwap;
using Solnet.Programs.TokenSwap.Models;
using Solnet.Rpc.Models;
using Solnet.Wallet;
using System.Collections.Generic;
using System.Linq;

namespace Solnet.Programs.Test
{
    [TestClass]
    public class TokenSwapProgramTest
    {
        private const string MnemonicWords =
            "route clerk disease box emerge airport loud waste attitude film army tray " +
            "forward deal onion eight catalog surface unit card window walnut wealth medal";

        private static readonly byte[] TokenSwapProgramIdBytes =
        {
            6,165,58,174,54,191,72,111,181,217,56,38,
            78,230,69,215,75,96,22,224,244,122,235,
            179,236,22,67,139,247,191,251,225
        };

        private static readonly byte[] ExpectedInitializeData =
        {
            0,254,1,0,0,0,0,0,0,0,100,0,0,0,0,0,0,0,1,0,0,0,0,0,
            0,0,100,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,0,0,0,0,0,0,
            0,1,0,0,0,0,0,0,0,232,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,
            0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        };

        private static readonly byte[] ExpectedSwapData =
        {
            1,128,26,6,0,0,0,0,0,32,179,129,0,0,0,0,0
        };

        private static readonly byte[] ExpectedDepositAllTokenTypesData =
        {
            2,4,0,0,0,0,0,0,0,32,179,129,0,0,0,0,0,160,15,0,0,0,0,0,0
        };

        private static readonly byte[] ExpectedWithdrawAllTokenTypesData =
        {
            3,4,0,0,0,0,0,0,0,160,15,0,0,0,0,0,0,32,179,129,0,0,0,0,0
        };

        private static readonly byte[] ExpectedDepositSingleTokenTypeExactAmountInData =
        {
            4,160,15,0,0,0,0,0,0,4,0,0,0,0,0,0,0
        };
        private static readonly byte[] ExpectedWithdrawSingleTokenTypeExactAmountOutData =
        {
            5,160,15,0,0,0,0,0,0,4,0,0,0,0,0,0,0
        };

        private const string InitializeMessage =
            "AgAHC1MuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft3/FflD5yxhXv/GyRPQxWneSI1" +
            "9VP2k43gUpVYG2jNHwarv91zFFZ0BDXh2dnixS0rka8rnVm8/lwluHEzfmVwaq9yV5EkRlspI5d" +
            "TBei2pTw72+yOOEUXqFwgg1djn1hXAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" +
            "AAAAAD0E4aXqh2tQhGa5IVDemLCaLk5I4fWxHtDzbxweno50QGqkt1zAcrZOVxGCNL6Xm7" +
            "NI3/Bm+44+nxDHxEdV6rYjoSyYQV+btxvbXHxDsERTxTz2CLMUCdl3qxnNxEiIzEl6yl4BybR" +
            "MuKQsQucwG8zcPF4h2aVMSq1AidCfnxnLgbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+" +
            "/wCpBqU6rja/SG+12TgmTuZF10tgFuD0euuz7BZDi/e/++Hmyh7pP4homUV4nZbFzDiNooTfV0" +
            "TICDNPFy0DXREIwgIEAgABNAAAAADAADAAAAAAAEQBAAAAAAAABqU6rja/SG+12TgmT" +
            "uZF10tgFuD0euuz7BZDi/e/++EKCAEFBgcCCAMJYwD9GQAAAAAAAAAQJwAAAAAAAAUAA" +
            "AAAAAAAECcAAAAAAAAAAAAAAAAAAAAAAAAAAAAAFAAAAAAAAABkAAAAAAAAAAAAA" +
            "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==";

        private const string SwapMessage =
            "AQAEC1MuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft37QqyFDtQcH7hIYXKOEvkCQa+" +
            "SmTK5A6OGMeeZooUoakBqpLdcwHK2TlcRgjS+l5uzSN/wZvuOPp8Qx8RHVeq2I6EsmEFfm7cb" +
            "21x8Q7BEU8U89gizFAnZd6sZzcRIiMxwQavoWObAlxFe84OJSfFUsLJIhR4Q2+v+4N9Vt58Vla" +
            "rv91zFFZ0BDXh2dnixS0rka8rnVm8/lwluHEzfmVwaiXrKXgHJtEy4pCxC5zAbzNw8XiHZpUxKr" +
            "UCJ0J+fGcu/FflD5yxhXv/GyRPQxWneSI19VP2k43gUpVYG2jNHwb0E4aXqh2tQhGa5IVDemL" +
            "CaLk5I4fWxHtDzbxweno50Qbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpBqU6rja/SG" +
            "+12TgmTuZF10tgFuD0euuz7BZDi/e/++H0144NBdw24rNWa3osyQqbSeyvVJGFXla9Rpj5nnnRRQ" +
            "EKCgcIAAECAwQFBgkRAQDKmjsAAAAAIKEHAAAAAAA=";

        private const string DepositAllTokenTypesMessage =
            "AQAEC1MuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft37QqyFDtQcH7hIYXKOEvkCQa+S" +
            "mTK5A6OGMeeZooUoanBBq+hY5sCXEV7zg4lJ8VSwskiFHhDb6/7g31W3nxWVgGqkt1zAcrZOV" +
            "xGCNL6Xm7NI3/Bm+44+nxDHxEdV6rYjoSyYQV+btxvbXHxDsERTxTz2CLMUCdl3qxnNxEiIzGr" +
            "v91zFFZ0BDXh2dnixS0rka8rnVm8/lwluHEzfmVwaq9yV5EkRlspI5dTBei2pTw72+yOOEUXqFwg" +
            "g1djn1hX/FflD5yxhXv/GyRPQxWneSI19VP2k43gUpVYG2jNHwb0E4aXqh2tQhGa5IVDemLCaLk" +
            "5I4fWxHtDzbxweno50Qbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpBqU6rja/SG+12" +
            "TgmTuZF10tgFuD0euuz7BZDi/e/++G/iXGArXvtQXqAznGhXSmATofHCuoBlpHxgPk4SfhBjwEKCg" +
            "cIAAECAwQFBgkZAkBCDwAAAAAAAOh2SBcAAAAA6HZIFwAAAA==";

        private const string WithdrawAllTokenTypesMessage =
            "AQAEDFMuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft3q7/dcxRWdAQ14dnZ4sUtK5GvK" +
            "51ZvP5cJbhxM35lcGqvcleRJEZbKSOXUwXotqU8O9vsjjhFF6hcIINXY59YVwGqkt1zAcrZOVxGC" +
            "NL6Xm7NI3/Bm+44+nxDHxEdV6rYjoSyYQV+btxvbXHxDsERTxTz2CLMUCdl3qxnNxEiIzHtCrI" +
            "UO1BwfuEhhco4S+QJBr5KZMrkDo4Yx55mihShqcEGr6FjmwJcRXvODiUnxVLCySIUeENvr/uD" +
            "fVbefFZWJespeAcm0TLikLELnMBvM3DxeIdmlTEqtQInQn58Zy78V+UPnLGFe/8bJE9DFad5Ij" +
            "X1U/aTjeBSlVgbaM0fBvQThpeqHa1CEZrkhUN6YsJouTkjh9bEe0PNvHB6ejnRBt324ddloZPZy+" +
            "FGzut5rBy0he1fWzeROoz1hX7/AKkGpTquNr9Ib7XZOCZO5kXXS2AW4PR667PsFkOL97/74RgZ" +
            "qeIKqmWN9s3Opx7A0mQO3EPmMmA+8ndUoI0JQ3gfAQsLCAkAAQIDBAUGBwoZA0BCDwAA" +
            "AAAA6AMAAAAAAADoAwAAAAAAAA==";

        private const string DepositSingleTokenTypeExactAmountInMessage =
            "AQAEClMuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft37QqyFDtQcH7hIYXKOEvkCQa+" +
            "SmTK5A6OGMeeZooUoakBqpLdcwHK2TlcRgjS+l5uzSN/wZvuOPp8Qx8RHVeq2I6EsmEFfm7c" +
            "b21x8Q7BEU8U89gizFAnZd6sZzcRIiMxq7/dcxRWdAQ14dnZ4sUtK5GvK51ZvP5cJbhxM35lc" +
            "GqvcleRJEZbKSOXUwXotqU8O9vsjjhFF6hcIINXY59YV/xX5Q+csYV7/xskT0MVp3kiNfVT9p" +
            "ON4FKVWBtozR8G9BOGl6odrUIRmuSFQ3piwmi5OSOH1sR7Q828cHp6OdEG3fbh12Whk9nL4" +
            "UbO63msHLSF7V9bN5E6jPWFfv8AqQalOq42v0hvtdk4Jk7mRddLYBbg9Hrrs+wWQ4v3v/vhzr" +
            "NmDrCfcB0Cg6zcl3Vo7qSZvl3ypatPmPfURasFfUABCQkGBwABAgMEBQgRBADKmjsAAAAA6A" +
            "MAAAAAAAA=";

        private const string WithdrawSingleTokenTypeExactAmountOutMessage =
            "AQAEC1MuM7pUYPM9siiE2WjcHJ6uhumh/A9CE2nvOtqmyft3q7/dcxRWdAQ14dnZ4sUtK5Gv" +
            "K51ZvP5cJbhxM35lcGqvcleRJEZbKSOXUwXotqU8O9vsjjhFF6hcIINXY59YVwGqkt1zAcrZOV" +
            "xGCNL6Xm7NI3/Bm+44+nxDHxEdV6rYjoSyYQV+btxvbXHxDsERTxTz2CLMUCdl3qxnNxEiIz" +
            "HtCrIUO1BwfuEhhco4S+QJBr5KZMrkDo4Yx55mihShqSXrKXgHJtEy4pCxC5zAbzNw8XiHZp" +
            "UxKrUCJ0J+fGcu/FflD5yxhXv/GyRPQxWneSI19VP2k43gUpVYG2jNHwb0E4aXqh2tQhGa5I" +
            "VDemLCaLk5I4fWxHtDzbxweno50Qbd9uHXZaGT2cvhRs7reawctIXtX1s3kTqM9YV+/wCpBq" +
            "U6rja/SG+12TgmTuZF10tgFuD0euuz7BZDi/e/++G6sYz49vuFr7rLN/dMfUEvpaHxP6DxaNZa" +
            "SUp0zrIUswEKCgcIAAECAwQFBgkRBUBCDwAAAAAAoIYBAAAAAAA=";

        [TestMethod]
        public void TestInitialize()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var tokenA = wallet.GetAccount(3);
            var tokenB = wallet.GetAccount(4);
            var poolMint = wallet.GetAccount(5);
            var poolFee = wallet.GetAccount(6);
            var poolToken = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().Initialize(
                tokenSwapAccount,
                tokenA.PublicKey,
                tokenB.PublicKey,
                poolMint.PublicKey,
                poolFee.PublicKey,
                poolToken.PublicKey,
                new Fees()
                {
                    TradeFeeNumerator = 1,
                    TradeFeeDenominator = 100,
                    OwnerWithrawFeeNumerator = 0,
                    OwnerWithrawFeeDenomerator = 1,
                    OwnerTradeFeeNumerator = 1,
                    OwnerTradeFeeDenomerator = 100,
                    HostFeeNumerator = 1,
                    HostFeeDenomerator = 1000,
                },
                SwapCurve.ConstantProduct
            );

            Assert.AreEqual(8, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedInitializeData, txInstruction.Data);
        }

        [TestMethod]
        public void TestSwap()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var userXfer = wallet.GetAccount(3);
            var source = wallet.GetAccount(4);
            var into = wallet.GetAccount(5);
            var from = wallet.GetAccount(6);
            var destination = wallet.GetAccount(7);
            var poolTokenMint = wallet.GetAccount(7);
            var fee = wallet.GetAccount(7);
            var hostFee = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().Swap(
                tokenSwapAccount,
                userXfer.PublicKey,
                source.PublicKey,
                into.PublicKey,
                from.PublicKey,
                destination.PublicKey,
                poolTokenMint.PublicKey,
                fee.PublicKey,
                hostFee.PublicKey,
                400_000,
                8_500_000
            );

            Assert.AreEqual(11, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedSwapData, txInstruction.Data);
        }

        [TestMethod]
        public void TestDepositAllTokenTypes()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var userXfer = wallet.GetAccount(3);
            var authA = wallet.GetAccount(4);
            var authB = wallet.GetAccount(5);
            var baseA = wallet.GetAccount(6);
            var baseB = wallet.GetAccount(7);
            var poolTokenMint = wallet.GetAccount(7);
            var poolAccount = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().DepositAllTokenTypes(
                tokenSwapAccount,
                userXfer.PublicKey,
                authA.PublicKey,
                authB.PublicKey,
                baseA.PublicKey,
                baseB.PublicKey,
                poolTokenMint.PublicKey,
                poolAccount.PublicKey,
                4,
                8_500_000,
                4_000
            );

            Assert.AreEqual(10, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedDepositAllTokenTypesData, txInstruction.Data);
        }

        [TestMethod]
        public void TestWithdrawAllTokenTypes()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var userXfer = wallet.GetAccount(3);
            var poolTokenMint = wallet.GetAccount(4);
            var sourcePoolAccount = wallet.GetAccount(4);
            var tokenAFrom = wallet.GetAccount(5);
            var tokenBFrom = wallet.GetAccount(6);
            var tokenATo = wallet.GetAccount(7);
            var tokenBTo = wallet.GetAccount(7);
            var feeAccount = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().WithdrawAllTokenTypes(
                tokenSwapAccount,
                userXfer.PublicKey,
                poolTokenMint.PublicKey,
                sourcePoolAccount.PublicKey,
                tokenAFrom.PublicKey,
                tokenBFrom.PublicKey,
                tokenATo.PublicKey,
                tokenBTo.PublicKey,
                feeAccount.PublicKey,
                4,
                4_000,
                8_500_000
            );

            Assert.AreEqual(11, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedWithdrawAllTokenTypesData, txInstruction.Data);
        }

        [TestMethod]
        public void TestDepositSingleTokenTypeExactAmountInTypes()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var userXfer = wallet.GetAccount(3);
            var tokenSource = wallet.GetAccount(4);
            var tokenA = wallet.GetAccount(5);
            var tokenB = wallet.GetAccount(6);
            var poolMint = wallet.GetAccount(7);
            var pool = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().DepositSingleTokenTypeExactAmountIn(
                tokenSwapAccount,
                userXfer.PublicKey,
                tokenSource.PublicKey,
                tokenA.PublicKey,
                tokenB.PublicKey,
                poolMint.PublicKey,
                pool.PublicKey,
                4_000,
                4
            );

            Assert.AreEqual(9, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedDepositSingleTokenTypeExactAmountInData, txInstruction.Data);
        }

        [TestMethod]
        public void TestWithdrawSingleTokenTypeExactAmountOutTypes()
        {
            var wallet = new Wallet.Wallet(MnemonicWords);

            var tokenSwapAccount = wallet.GetAccount(1);
            var userXfer = wallet.GetAccount(3);
            var poolMint = wallet.GetAccount(4);
            var sourcePool = wallet.GetAccount(5);
            var tokenASource = wallet.GetAccount(6);
            var tokenBSource = wallet.GetAccount(6);
            var userToken = wallet.GetAccount(7);
            var feeAccount = wallet.GetAccount(7);

            var txInstruction = new TokenSwapProgram().WithdrawSingleTokenTypeExactAmountOut(
                tokenSwapAccount,
                userXfer.PublicKey,
                poolMint.PublicKey,
                sourcePool.PublicKey,
                tokenASource.PublicKey,
                tokenBSource.PublicKey,
                userToken.PublicKey,
                feeAccount.PublicKey,
                4_000,
                4
            );

            Assert.AreEqual(10, txInstruction.Keys.Count);
            CollectionAssert.AreEqual(TokenSwapProgramIdBytes, txInstruction.ProgramId);
            CollectionAssert.AreEqual(ExpectedWithdrawSingleTokenTypeExactAmountOutData, txInstruction.Data);
        }

        [TestMethod]
        public void InitializeDecodeTest()
        {
            Message msg = Message.Deserialize(InitializeMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(2, decodedInstructions.Count);
            Assert.AreEqual("[0] 11111111111111111111111111111111:System Program:Create Account\n[0] [[Owner Account, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[New Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Amount, 3145920],[Space, 324]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[0].ToString());
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Initialize Swap\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[Token A Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Token B Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[Pool Token Mint, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[Pool Token Fee Account, 3Z24fqykBPn1wNSXGz7SA5MXqGGk3DPSDpmxQoERMHrM],[Pool Token Account, CosUN9gxk8M6gdSDHYvaKKKCbX2VL73z1mJ66tYFsnSA],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Nonce, 253],[Trade Fee Numerator, 25],[Trade Fee Denominator, 10000],[Owner Trade Fee Numerator, 5],[Owner Trade Fee Denominator, 10000],[Owner Withraw Fee Numerator, 0],[Owner Withraw Fee Denominator, 0],[Host Fee Numerator, 20],[Host Fee Denominator, 100],[Curve Type, 0]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[1].ToString());
        }

        [TestMethod]
        public void SwapDecodeTest()
        {
            Message msg = Message.Deserialize(SwapMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Swap\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[User Transfer Authority, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[User Source Account, GxK5rLRGx1AnE9BZzQBP6SVenavuZqRUXbE6QTzL3jjW],[Token Base Into Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Token Base From Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[User Destination Account, DzVbjXqE9oFMJ4dWa9PqCA2bmiARtSURpmijux3PkC45],[Pool Token Mint, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[Fee Account, 3Z24fqykBPn1wNSXGz7SA5MXqGGk3DPSDpmxQoERMHrM],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Amount In, 1000000000],[Amount Out, 500000]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[0].ToString());
        }

        [TestMethod]
        public void DepositAllTokenTypesDecodeTest()
        {
            Message msg = Message.Deserialize(DepositAllTokenTypesMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Deposit Both\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[User Transfer Authority, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[User Token A Account, GxK5rLRGx1AnE9BZzQBP6SVenavuZqRUXbE6QTzL3jjW],[User Token B Account, DzVbjXqE9oFMJ4dWa9PqCA2bmiARtSURpmijux3PkC45],[Pool Token A Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Pool Token B Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[Pool Token Mint, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[User Pool Token Account, CosUN9gxk8M6gdSDHYvaKKKCbX2VL73z1mJ66tYFsnSA],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Pool Tokens, 1000000],[Max Token A, 100000000000],[Max Token B, 100000000000]]\n[0] InnerInstructions (0)\n",
               decodedInstructions[0].ToString());
        }

        [TestMethod]
        public void WithdrawAllTokenTypesDecodeTest()
        {
            Message msg = Message.Deserialize(WithdrawAllTokenTypesMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Withdraw Both\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[User Transfer Authority, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[Pool Token Account, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[User Pool Token Account, CosUN9gxk8M6gdSDHYvaKKKCbX2VL73z1mJ66tYFsnSA],[Pool Token A Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Pool Token B Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[User Token A Account, GxK5rLRGx1AnE9BZzQBP6SVenavuZqRUXbE6QTzL3jjW],[User Token B Account, DzVbjXqE9oFMJ4dWa9PqCA2bmiARtSURpmijux3PkC45],[Fee Account, 3Z24fqykBPn1wNSXGz7SA5MXqGGk3DPSDpmxQoERMHrM],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Pool Tokens, 1000000],[Min Token A, 1000],[Min Token B, 1000]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[0].ToString());
        }

        [TestMethod]
        public void DepositSingleTokenTypeExactAmountInDecodeTest()
        {
            Message msg = Message.Deserialize(DepositSingleTokenTypeExactAmountInMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Deposit Single\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[User Transfer Authority, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[User Source Token Account, GxK5rLRGx1AnE9BZzQBP6SVenavuZqRUXbE6QTzL3jjW],[Token A Swap Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Token B Swap Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[Pool Mint Account, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[User Pool Token Account, CosUN9gxk8M6gdSDHYvaKKKCbX2VL73z1mJ66tYFsnSA],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Source Token Amount, 1000000000],[Min Pool Token Amount, 1000]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[0].ToString());
        }

        [TestMethod]
        public void WithdrawSingleTokenTypeExactAmountOutDecodeTest()
        {
            Message msg = Message.Deserialize(WithdrawSingleTokenTypeExactAmountOutMessage);
            List<DecodedInstruction> decodedInstructions = InstructionDecoder.DecodeInstructions(msg);

            Assert.AreEqual(1, decodedInstructions.Count);
            Assert.AreEqual("[0] SwaPpA9LAaLfeLi3a68M4DjnLqgtticKg6CnyNwgAC8:Token Swap Program:Withdraw Single\n[0] [[Token Swap Account, Hz3UWwAR4z7TZmzMW2TFjjzDtxEveiZZbJ4sg1LEuvKo],[Swap Authority, HRmkKfXbHcvNhWHw47zqoexKiLHmowR8o7hdwwWdaHoW],[User Transfer Authority, 6bhhceZToGG9RsTe1nfNFXEMjavhj6CV55EsvearAt2z],[Pool Mint Account, CZSQMnD4jTvRfEuApDAmjWvz1AWpFpXqoePPXwZpmk1F],[User Pool Token Account, CosUN9gxk8M6gdSDHYvaKKKCbX2VL73z1mJ66tYFsnSA],[Token A Swap Account, 7WGJswQpwuNePUiEFBqCMKnGcpkNoX7fFeAdM16o1wV],[Token B Swap Account, AbLFYgniLdGWikGJX3dT4iTWoX1FbFBwu2sjGDQN7nfa],[User Token Account, GxK5rLRGx1AnE9BZzQBP6SVenavuZqRUXbE6QTzL3jjW],[Fee Account, 3Z24fqykBPn1wNSXGz7SA5MXqGGk3DPSDpmxQoERMHrM],[Token Program ID, TokenkegQfeZyiNwAJbNbGKPFXCWuBvf9Ss623VQ5DA],[Destination Token Amount, 1000000],[Max Pool Token Amount, 100000]]\n[0] InnerInstructions (0)\n",
                    decodedInstructions[0].ToString());
        }
    }
}