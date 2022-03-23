// unset

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Sol.Unity.Wallet.Bip39;
using Sol.Unity.Wallet.Utilities;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Sol.Unity.Wallet.Test
{
    [TestClass]
    public class Bip39Test
    {

        [TestMethod]
        public void CanGenerateMnemonicOfSpecificLength()
        {
            foreach (var count in new[] { WordCount.Twelve, WordCount.TwentyFour, WordCount.TwentyOne, WordCount.Fifteen, WordCount.Eighteen })
            {
                Assert.AreEqual((int)count, new Mnemonic(WordList.English, count).Words.Length);
            }
        }

        [TestMethod]
        public void CanDetectBadChecksum()
        {
            var mnemonic = new Mnemonic("turtle front uncle idea crush write shrug there lottery flower risk shell", WordList.English);
            Assert.IsTrue(mnemonic.IsValidChecksum);
            mnemonic = new Mnemonic("front front uncle idea crush write shrug there lottery flower risk shell", WordList.English);
            Assert.IsFalse(mnemonic.IsValidChecksum);
        }

        [TestMethod]
        public void CanNormalizeMnemonicString()
        {
            var mnemonic = new Mnemonic("turtle front uncle idea crush write shrug there lottery flower risk shell", WordList.English);
            var mnemonic2 = new Mnemonic("turtle    front	uncle　 idea crush write shrug there lottery flower risk shell", WordList.English);
            Assert.AreEqual(mnemonic.ToString(), mnemonic2.ToString());
        }

        [TestMethod]
        public void EnglishTest()
        {
            var test = JsonDocument.Parse(File.ReadAllText("Resources/Bip39Vectors.json")).RootElement;

            foreach (var unitTest in test.EnumerateArray())
            {
                var entropy = BitConverter.ToString(Encoding.Default.GetBytes(unitTest[0].ToString())).ToLowerInvariant().Replace("-", "");
                string mnemonicStr = unitTest[1].ToString();
                string seed = unitTest[2].ToString();
                var mnemonic = new Mnemonic(mnemonicStr, WordList.English);
                Assert.IsTrue(mnemonic.IsValidChecksum);
                Assert.AreEqual(seed, BitConverter.ToString(mnemonic.DeriveSeed("TREZOR")).ToLowerInvariant().Replace("-", ""));
            }
        }

        [TestMethod]
        public void CanReturnTheListOfWords()
        {
            var lang = WordList.English;
            var words = lang.GetWords();
            int i;
            foreach (var word in words)
            {
                Assert.IsTrue(lang.WordExists(word, out i));
                Assert.IsTrue(i >= 0);
            }
        }

        [TestMethod]
        public void KdTableCanNormalize()
        {
            var input = "あおぞら";
            var expected = "あおぞら";
            Assert.IsFalse(input == expected);
            Assert.AreEqual(expected, KdTable.NormalizeKd(input));
        }

        [TestMethod]
        public void JapaneseTest()
        {
            var test = JsonDocument.Parse(File.ReadAllText("Resources/Bip39Japanese.json")).RootElement;

            foreach (var unitTest in test.EnumerateArray())
            {
                string mnemonicStr = unitTest.GetProperty("mnemonic").ToString();
                string seed = unitTest.GetProperty("seed").ToString();
                string passphrase = unitTest.GetProperty("passphrase").ToString();
                var mnemonic = new Mnemonic(mnemonicStr, WordList.Japanese);
                Assert.IsTrue(mnemonic.IsValidChecksum);
                Assert.AreEqual(seed, BitConverter.ToString(mnemonic.DeriveSeed(passphrase)).ToLowerInvariant().Replace("-", ""));
                Assert.IsTrue(mnemonic.IsValidChecksum);
            }
        }

        [TestMethod]
        public void TestKnownEnglish()
        {
            Assert.AreEqual(Language.English, WordList.AutoDetectLanguage(new string[] { "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "abandon", "about" }));
        }

        [TestMethod]
        public void TestKnownJapenese()
        {
            Assert.AreEqual(Language.Japanese, WordList.AutoDetectLanguage(new string[] { "あいこくしん", "あいさつ", "あいだ", "あおぞら", "あかちゃん", "あきる", "あけがた", "あける", "あこがれる", "あさい", "あさひ", "あしあと", "あじわう", "あずかる", "あずき", "あそぶ", "あたえる", "あたためる", "あたりまえ", "あたる", "あつい", "あつかう", "あっしゅく", "あつまり", "あつめる", "あてな", "あてはまる", "あひる", "あぶら", "あぶる", "あふれる", "あまい", "あまど", "あまやかす", "あまり", "あみもの", "あめりか" }));
        }

        [TestMethod]
        public void TestKnownSpanish()
        {
            Assert.AreEqual(Language.Spanish, WordList.AutoDetectLanguage(new string[] { "yoga", "yogur", "zafiro", "zanja", "zapato", "zarza", "zona", "zorro", "zumo", "zurdo" }));
        }

        [TestMethod]
        public void TestKnownFrench()
        {
            Assert.AreEqual(Language.French, WordList.AutoDetectLanguage(new string[] { "abusif", "antidote" }));
        }

        [TestMethod]
        public void TestKnownChineseSimplified()
        {
            Assert.AreEqual(Language.ChineseSimplified, WordList.AutoDetectLanguage(new string[] { "的", "一", "是", "在", "不", "了", "有", "和", "人", "这" }));
        }

        [TestMethod]
        public void TestKnownChineseTraditional()
        {
            Assert.AreEqual(Language.ChineseTraditional, WordList.AutoDetectLanguage(new string[] { "的", "一", "是", "在", "不", "了", "有", "和", "載" }));
        }

        [TestMethod]
        public void TestKnownUnknown()
        {
            Assert.AreEqual(Language.Unknown, WordList.AutoDetectLanguage(new string[] { "gffgfg", "khjkjk", "kjkkj" }));
        }
    }
}