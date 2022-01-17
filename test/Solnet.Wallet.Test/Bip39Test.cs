// unset

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Solnet.Wallet.Bip39;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Solnet.Wallet.Test
{
    [TestClass]
    public class Bip39Test
    {
        [TestMethod]
        public void CanGenerateMnemonicOfSpecificLength()
        {
            foreach (WordCount count in new[]
                     {
                         WordCount.Twelve, WordCount.TwentyFour, WordCount.TwentyOne, WordCount.Fifteen,
                         WordCount.Eighteen
                     })
            {
                Assert.AreEqual((int)count, new Mnemonic(WordList.English, count).Words.Length);
            }
        }

        [TestMethod]
        public void CanDetectBadChecksum()
        {
            Mnemonic mnemonic =
                new Mnemonic("turtle front uncle idea crush write shrug there lottery flower risk shell",
                    WordList.English);
            Assert.IsTrue(mnemonic.IsValidChecksum);
            mnemonic = new Mnemonic("front front uncle idea crush write shrug there lottery flower risk shell",
                WordList.English);
            Assert.IsFalse(mnemonic.IsValidChecksum);
        }

        [TestMethod]
        public void CanNormalizeMnemonicString()
        {
            Mnemonic mnemonic =
                new Mnemonic("turtle front uncle idea crush write shrug there lottery flower risk shell",
                    WordList.English);
            Mnemonic mnemonic2 =
                new Mnemonic("turtle    front	uncle　 idea crush write shrug there lottery flower risk shell",
                    WordList.English);
            Assert.AreEqual(mnemonic.ToString(), mnemonic2.ToString());
        }

        [TestMethod]
        public void EnglishTest()
        {
            JsonElement test = JsonDocument.Parse(File.ReadAllText("Resources/Bip39Vectors.json")).RootElement;

            foreach (JsonElement unitTest in test.EnumerateArray())
            {
                string entropy = BitConverter.ToString(Encoding.Default.GetBytes(unitTest[0].ToString()))
                    .ToLowerInvariant().Replace("-", "");
                string mnemonicStr = unitTest[1].ToString();
                string seed = unitTest[2].ToString();
                Mnemonic mnemonic = new Mnemonic(mnemonicStr, WordList.English);
                Assert.IsTrue(mnemonic.IsValidChecksum);
                Assert.AreEqual(seed,
                    BitConverter.ToString(mnemonic.DeriveSeed("TREZOR")).ToLowerInvariant().Replace("-", ""));
            }
        }

        [TestMethod]
        public void CanReturnTheListOfWords()
        {
            WordList lang = WordList.English;
            IEnumerable<string> words = lang.GetWords();
            int i;
            foreach (string word in words)
            {
                Assert.IsTrue(lang.WordExists(word, out i));
                Assert.IsTrue(i >= 0);
            }
        }

        [TestMethod]
        public void KdTableCanNormalize()
        {
            string input = "あおぞら";
            string expected = "あおぞら";
            Assert.IsFalse(input == expected);
            Assert.AreEqual(expected, KdTable.NormalizeKd(input));
        }

        [TestMethod]
        public void JapaneseTest()
        {
            JsonElement test = JsonDocument.Parse(File.ReadAllText("Resources/Bip39Japanese.json")).RootElement;

            foreach (JsonElement unitTest in test.EnumerateArray())
            {
                string mnemonicStr = unitTest.GetProperty("mnemonic").ToString();
                string seed = unitTest.GetProperty("seed").ToString();
                string passphrase = unitTest.GetProperty("passphrase").ToString();
                Mnemonic mnemonic = new Mnemonic(mnemonicStr, WordList.Japanese);
                Assert.IsTrue(mnemonic.IsValidChecksum);
                Assert.AreEqual(seed,
                    BitConverter.ToString(mnemonic.DeriveSeed(passphrase)).ToLowerInvariant().Replace("-", ""));
                Assert.IsTrue(mnemonic.IsValidChecksum);
            }
        }

        [TestMethod]
        public void TestKnownEnglish()
        {
            Assert.AreEqual(Language.English,
                WordList.AutoDetectLanguage(new[]
                {
                    "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon",
                    "abandon", "abandon", "abandon", "about"
                }));
        }

        [TestMethod]
        public void TestKnownJapenese()
        {
            Assert.AreEqual(Language.Japanese,
                WordList.AutoDetectLanguage(new[]
                {
                    "あいこくしん", "あいさつ", "あいだ", "あおぞら", "あかちゃん", "あきる", "あけがた", "あける", "あこがれる", "あさい", "あさひ",
                    "あしあと", "あじわう", "あずかる", "あずき", "あそぶ", "あたえる", "あたためる", "あたりまえ", "あたる", "あつい", "あつかう",
                    "あっしゅく", "あつまり", "あつめる", "あてな", "あてはまる", "あひる", "あぶら", "あぶる", "あふれる", "あまい", "あまど", "あまやかす",
                    "あまり", "あみもの", "あめりか"
                }));
        }

        [TestMethod]
        public void TestKnownSpanish()
        {
            Assert.AreEqual(Language.Spanish,
                WordList.AutoDetectLanguage(new[]
                {
                    "yoga", "yogur", "zafiro", "zanja", "zapato", "zarza", "zona", "zorro", "zumo", "zurdo"
                }));
        }

        [TestMethod]
        public void TestKnownFrench()
        {
            Assert.AreEqual(Language.French, WordList.AutoDetectLanguage(new[] {"abusif", "antidote"}));
        }

        [TestMethod]
        public void TestKnownChineseSimplified()
        {
            Assert.AreEqual(Language.ChineseSimplified,
                WordList.AutoDetectLanguage(new[] {"的", "一", "是", "在", "不", "了", "有", "和", "人", "这"}));
        }

        [TestMethod]
        public void TestKnownChineseTraditional()
        {
            Assert.AreEqual(Language.ChineseTraditional,
                WordList.AutoDetectLanguage(new[] {"的", "一", "是", "在", "不", "了", "有", "和", "載"}));
        }

        [TestMethod]
        public void TestKnownUnknown()
        {
            Assert.AreEqual(Language.Unknown, WordList.AutoDetectLanguage(new[] {"gffgfg", "khjkjk", "kjkkj"}));
        }
    }
}