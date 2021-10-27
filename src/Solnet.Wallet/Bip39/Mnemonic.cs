using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Solnet.Wallet.Utilities;
using System;
using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Solnet.Wallet.Bip39
{
    /// <summary>
    /// A .NET implementation of the Bitcoin Improvement Proposal - 39 (BIP39)
    /// BIP39 specification used as reference located here: https://github.com/bitcoin/bips/blob/master/bip-0039.mediawiki
    /// This implementation was taken from NBitcoin: https://github.com/MetacoSA/NBitcoin/
    /// </summary>
    [DebuggerDisplay("Mnemonic = {" + nameof(_mnemonic) + "}")]
    public class Mnemonic
    {
        /// <summary>
        /// Initialize a mnemonic from the given string and wordList type.
        /// </summary>
        /// <param name="mnemonic">The mnemonic string.</param>
        /// <param name="wordList">The word list type.</param>
        /// <exception cref="ArgumentNullException">Thrown when the mnemonic string is null.</exception>
        /// <exception cref="FormatException">Thrown when the word count of the mnemonic is invalid.</exception>
        public Mnemonic(string mnemonic, WordList wordList = null)
        {
            if (mnemonic == null)
                throw new ArgumentNullException(nameof(mnemonic));
            _mnemonic = mnemonic.Trim();

            wordList ??= WordList.AutoDetect(mnemonic) ?? WordList.English;

            string[] words = mnemonic.Split((char[])null, StringSplitOptions.RemoveEmptyEntries);
            _mnemonic = string.Join(wordList.Space.ToString(), words);

            //if the sentence is not at least 12 characters or cleanly divisible by 3, it is bad!
            if (!CorrectWordCount(words.Length))
            {
                throw new FormatException("Word count should be 12,15,18,21 or 24");
            }
            Words = words;
            WordList = wordList;
            Indices = wordList.ToIndices(words);
        }

        /// <summary>
        /// Generate a mnemonic
        /// </summary>
        /// <param name="wordList">The word list of the mnemonic.</param>
        /// <param name="entropy">The entropy.</param>
        private Mnemonic(WordList wordList, byte[] entropy = null)
        {
            wordList ??= WordList.English;
            WordList = wordList;
            entropy ??= RandomUtils.GetBytes(32);

            int i = Array.IndexOf(EntArray, entropy.Length * 8);
            if (i == -1)
                throw new ArgumentException("The length for entropy should be " + string.Join(",", EntArray) + " bits", nameof(entropy));

            int cs = CsArray[i];
            byte[] checksum = Utils.Sha256(entropy);
            BitWriter entropyResult = new();

            entropyResult.Write(entropy);
            entropyResult.Write(checksum, cs);
            Indices = entropyResult.ToIntegers();
            Words = WordList.GetWords(Indices);
            _mnemonic = WordList.GetSentence(Indices);
        }

        /// <summary>
        /// Initialize a mnemonic from the given word list and word count..
        /// </summary>
        /// <param name="wordList">The word list.</param>
        /// <param name="wordCount">The word count.</param>
        public Mnemonic(WordList wordList, WordCount wordCount) : this(wordList, GenerateEntropy(wordCount)) { }

        /// <summary>
        /// Generate entropy for the given word count.
        /// </summary>
        /// <param name="wordCount"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown when the word count is invalid.</exception>
        private static byte[] GenerateEntropy(WordCount wordCount)
        {
            int ms = (int)wordCount;
            if (!CorrectWordCount(ms))
                throw new ArgumentException("Word count should be 12,15,18,21 or 24", nameof(wordCount));
            int i = Array.IndexOf(MsArray, (int)wordCount);
            return RandomUtils.GetBytes(EntArray[i] / 8);
        }

        /// <summary>
        /// The word count array.
        /// </summary>
        private static readonly int[] MsArray = { 12, 15, 18, 21, 24 };

        /// <summary>
        /// The bit count array.
        /// </summary>
        private static readonly int[] CsArray = { 4, 5, 6, 7, 8 };

        /// <summary>
        /// The entropy value array.
        /// </summary>
        private static readonly int[] EntArray = { 128, 160, 192, 224, 256 };

        /// <summary>
        /// Whether the checksum of the mnemonic is valid.
        /// </summary>
        private bool? _isValidChecksum;

        /// <summary>
        /// Whether the checksum of the mnemonic is valid.
        /// </summary>
        public bool IsValidChecksum
        {
            get
            {
                if (_isValidChecksum != null)
                {
                    return _isValidChecksum.Value;
                }

                int i = Array.IndexOf(MsArray, Indices.Length);
                int cs = CsArray[i];
                int ent = EntArray[i];

                BitWriter writer = new();
                BitArray bits = WordList.ToBits(Indices);
                writer.Write(bits, ent);
                byte[] entropy = writer.ToBytes();
                byte[] checksum = Utils.Sha256(entropy);

                writer.Write(checksum, cs);
                int[] expectedIndices = writer.ToIntegers();
                _isValidChecksum = expectedIndices.SequenceEqual(Indices);
                return _isValidChecksum.Value;
            }
        }

        /// <summary>
        /// Whether the word count is correct.
        /// </summary>
        /// <param name="ms">The number of words.</param>
        /// <returns>True if it is, otherwise false.</returns>
        private static bool CorrectWordCount(int ms)
        {
            return MsArray.Any(_ => _ == ms);
        }

        /// <summary>
        /// The word list.
        /// </summary>
        public WordList WordList { get; }

        /// <summary>
        /// The indices.
        /// </summary>
        public int[] Indices { get; }

        /// <summary>
        /// The words of the mnemonic.
        /// </summary>
        public string[] Words { get; }

        /// <summary>
        /// Utf8 encoding.
        /// </summary>
        private static readonly Encoding _noBomutf8 = new UTF8Encoding(false);

        /// <summary>
        /// Derives the mnemonic seed.
        /// </summary>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>The seed.</returns>
        public byte[] DeriveSeed(string passphrase = null)
        {
            passphrase ??= "";
            byte[] salt = Concat(_noBomutf8.GetBytes("mnemonic"), Normalize(passphrase));
            byte[] bytes = Normalize(_mnemonic);

            return GenerateSeed(bytes, salt);
        }

        /// <summary>
        /// Generate the seed using pbkdf with sha 512.
        /// </summary>
        /// <param name="password">The password to derive the key from.</param>
        /// <param name="salt">The salt to use for key derivation.</param>
        /// <returns>The derived key.</returns>
        private static byte[] GenerateSeed(byte[] password, byte[] salt)
        {
            Pkcs5S2ParametersGenerator gen = new(new Sha512Digest());
            gen.Init(password, salt, 2048);
            return ((KeyParameter)gen.GenerateDerivedParameters(512)).GetKey();
        }

        /// <summary>
        /// Get the normalized the string as a byte array.
        /// </summary>
        /// <param name="str">The string to normalize.</param>
        /// <returns>The byte array.</returns>
        private static byte[] Normalize(string str)
        {
            return _noBomutf8.GetBytes(NormalizeString(str));
        }

        /// <summary>
        /// Normalize the string.
        /// </summary>
        /// <param name="word">The string to normalize.</param>
        /// <returns>The normalized string.</returns>
        internal static string NormalizeString(string word)
        {
            return !SupportOsNormalization() ? KdTable.NormalizeKd(word) : word.Normalize(NormalizationForm.FormKD);
        }

        /// <summary>
        /// Whether the OS normalization is supported.
        /// </summary>
        private static bool? _supportOsNormalization;

        /// <summary>
        /// Checks for OS normalization support.
        /// </summary>
        /// <returns>True if available, otherwise false.</returns>
        private static bool SupportOsNormalization()
        {
            if (_supportOsNormalization != null)
            {
                return _supportOsNormalization.Value;
            }

            const string notNormalized = "あおぞら";
            const string normalized = "あおぞら";

            if (notNormalized.Equals(normalized, StringComparison.Ordinal))
            {
                _supportOsNormalization = false;
            }
            else
            {
                try
                {
                    _supportOsNormalization = notNormalized.Normalize(NormalizationForm.FormKD).Equals(normalized, StringComparison.Ordinal);
                }
                catch { _supportOsNormalization = false; }
            }
            return _supportOsNormalization.Value;
        }

        /// <summary>
        /// Concatenate an array of bytes.
        /// </summary>
        /// <param name="source1">The first array.</param>
        /// <param name="source2">The second array.</param>
        /// <returns>The concatenated array of bytes.</returns>
        private static byte[] Concat(byte[] source1, byte[] source2)
        {
            //Most efficient way to merge two arrays this according to http://stackoverflow.com/questions/415291/best-way-to-combine-two-or-more-byte-arrays-in-c-sharp
            byte[] buffer = new byte[source1.Length + source2.Length];
            Buffer.BlockCopy(source1, 0, buffer, 0, source1.Length);
            Buffer.BlockCopy(source2, 0, buffer, source1.Length, source2.Length);

            return buffer;
        }

        /// <summary>
        /// The mnemonic string.
        /// </summary>
        private readonly string _mnemonic;

        /// <summary>
        /// Gets the mnemonic string.
        /// </summary>
        /// <returns>The string.</returns>
        public override string ToString()
        {
            return _mnemonic;
        }
    }
}