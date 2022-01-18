using Solnet.Wallet.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solnet.Wallet.Bip39
{
    /// <summary>
    /// Implements the word list functionality.
    /// </summary>
    public class WordList
    {
        /// <summary>
        /// Initialize the static word list instance.
        /// </summary>
        static WordList()
        {
            WordlistSource = new HardcodedWordlistSource();
        }

        /// <summary>
        /// The japanese word list.
        /// </summary>
        private static WordList _japanese;

        /// <summary>
        /// The japanese word list.
        /// </summary>
        public static WordList Japanese => _japanese ??= LoadWordList(Language.Japanese).Result;

        /// <summary>
        /// The simplified chinese word list.
        /// </summary>
        private static WordList _chineseSimplified;

        /// <summary>
        /// The simplified chinese word list.
        /// </summary>
        public static WordList ChineseSimplified => _chineseSimplified ??= LoadWordList(Language.ChineseSimplified).Result;

        /// <summary>
        /// The traditional chinese word list.
        /// </summary>
        private static WordList _chineseTraditional;

        /// <summary>
        /// The traditional chinese word list.
        /// </summary>
        public static WordList ChineseTraditional => _chineseTraditional ??= LoadWordList(Language.ChineseTraditional).Result;

        /// <summary>
        /// The spanish word list.
        /// </summary>
        private static WordList _spanish;

        /// <summary>
        /// The spanish word list.
        /// </summary>
        public static WordList Spanish => _spanish ??= LoadWordList(Language.Spanish).Result;

        /// <summary>
        /// The english word list.
        /// </summary>
        private static WordList _english;

        /// <summary>
        /// The english word list.
        /// </summary>
        public static WordList English => _english ??= LoadWordList(Language.English).Result;

        /// <summary>
        /// The french word list.
        /// </summary>
        private static WordList _french;

        /// <summary>
        /// The french word list.
        /// </summary>
        public static WordList French => _french ??= LoadWordList(Language.French).Result;

        /// <summary>
        /// The brazilian portuguese word list.
        /// </summary>
        private static WordList _portugueseBrazil;

        /// <summary>
        /// The brazilian portuguese word list.
        /// </summary>
        public static WordList PortugueseBrazil => _portugueseBrazil ??= LoadWordList(Language.PortugueseBrazil).Result;

        /// <summary>
        /// The czech word list.
        /// </summary>
        private static WordList _czech;

        /// <summary>
        /// The czech word list.
        /// </summary>
        public static WordList Czech => _czech ??= LoadWordList(Language.Czech).Result;

        /// <summary>
        /// Load the word list for the given language. This operation is asynchronous.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>A task which returns the word list.</returns>
        public static Task<WordList> LoadWordList(Language language)
        {
            string name = GetLanguageFileName(language);
            return LoadWordList(name);
        }

        /// <summary>
        /// Gets the name of the file for the given language.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns>The file name.</returns>
        /// <exception cref="NotSupportedException">Thrown when the language is not supported.</exception>
        private static string GetLanguageFileName(Language language)
        {
            string name = language switch
            {
                Language.ChineseTraditional => "chinese_traditional",
                Language.ChineseSimplified => "chinese_simplified",
                Language.English => "english",
                Language.Japanese => "japanese",
                Language.Spanish => "spanish",
                Language.French => "french",
                Language.PortugueseBrazil => "portuguese_brazil",
                Language.Czech => "czech",
                Language.Unknown => throw new NotSupportedException(language.ToString()),
                _ => throw new NotSupportedException(language.ToString())
            };
            return name;
        }

        /// <summary>
        /// The loaded word lists.
        /// </summary>
        private static readonly Dictionary<string, WordList> LoadedLists = new();

        /// <summary>
        /// Loads a word list by name.
        /// </summary>
        /// <param name="name">The name of the word list.</param>
        /// <returns>A task which returns the word list.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the word list name is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the word list source is not initialized.</exception>
        private static async Task<WordList> LoadWordList(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            WordList result;
            lock (LoadedLists)
            {
                LoadedLists.TryGetValue(name, out result);
            }
            if (result != null)
                return await Task.FromResult(result).ConfigureAwait(false);

            if (WordlistSource == null)
                throw new InvalidOperationException("WordList.WordlistSource is not initialized, could not fetch word list.");
            result = await WordlistSource.LoadAsync(name).ConfigureAwait(false);

            if (result != null)
                lock (LoadedLists)
                {
                    LoadedLists.AddOrReplace(name, result);
                }

            return result;
        }

        /// <summary>
        /// The word list source.
        /// </summary>
        private static IWordlistSource WordlistSource { get; }

        /// <summary>
        /// The words of the word list.
        /// </summary>
        private readonly string[] _words;

        /// <summary>
        /// Constructor used by inheritance only.
        /// </summary>
        /// <param name="words">The words to be used in the wordlist</param>
        /// <param name="space">The words to be used in the wordlist</param>
        /// <param name="name">The words to be used in the wordlist</param>
        public WordList(IEnumerable<string> words, char space, string name)
        {
            _words = words
                        .Select(Mnemonic.NormalizeString)
                        .ToArray();
            _name = name;
            Space = space;
        }

        /// <summary>
        /// The name of the word list.
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// The space character of the word list.
        /// </summary>
        public char Space { get; }

        /// <summary>
        /// Method to determine if word exists in word list, great for auto language detection
        /// </summary>
        /// <param name="word">The word to check for existence</param>
        /// <param name="index">The index of the word.</param>
        /// <returns>True if it exists, otherwise false.</returns>
        public bool WordExists(string word, out int index)
        {
            word = Mnemonic.NormalizeString(word);
            if (_words.Contains(word))
            {
                index = Array.IndexOf(_words, word);
                return true;
            }

            //index -1 means word is not in wordlist
            index = -1;
            return false;
        }

        /// <summary>
        /// Returns a string containing the word at the specified index of the wordlist
        /// </summary>
        /// <param name="index">Index of word to return</param>
        /// <returns>Word</returns>
        private string GetWordAtIndex(int index)
        {
            return _words[index];
        }

        /// <summary>
        /// The number of all the words in the wordlist
        /// </summary>
        public int WordCount => _words.Length;

        /// <summary>
        /// Auto detects the language of the word list.
        /// </summary>
        /// <param name="sentence">The sentence to detect language from.</param>
        /// <returns>The word list.</returns>
        public static WordList AutoDetect(string sentence)
        {
            return LoadWordList(AutoDetectLanguage(sentence)).Result;
        }

        /// <summary>
        /// Auto detects the language of the word list given as an enumerable of strings.
        /// </summary>
        /// <param name="words">The sentence to detect language from.</param>
        /// <returns>The language.</returns>
        public static Language AutoDetectLanguage(IEnumerable<string> words)
        {
            List<int> languageCount = new(new[] { 0, 0, 0, 0, 0, 0, 0, 0 });

            foreach (string s in words)
            {
                if (English.WordExists(s, out int _))
                {
                    //english is at 0
                    languageCount[0]++;
                }

                if (Japanese.WordExists(s, out int _))
                {
                    //japanese is at 1
                    languageCount[1]++;
                }

                if (Spanish.WordExists(s, out int _))
                {
                    //spanish is at 2
                    languageCount[2]++;
                }

                if (ChineseSimplified.WordExists(s, out int _))
                {
                    //chinese simplified is at 3
                    languageCount[3]++;
                }

                if (ChineseTraditional.WordExists(s, out int _) && !ChineseSimplified.WordExists(s, out int _))
                {
                    //chinese traditional is at 4
                    languageCount[4]++;
                }
                if (French.WordExists(s, out int _))
                {
                    languageCount[5]++;
                }

                if (PortugueseBrazil.WordExists(s, out int _))
                {
                    //portuguese_brazil is at 6
                    languageCount[6]++;
                }

                if (Czech.WordExists(s, out int _))
                {
                    //czech is at 7
                    languageCount[7]++;
                }
            }

            //no hits found for any language unknown
            if (languageCount.Max() == 0)
            {
                return Language.Unknown;
            }

            return languageCount.IndexOf(languageCount.Max()) switch
            {
                0 => Language.English,
                1 => Language.Japanese,
                2 => Language.Spanish,
                3 when languageCount[4] > 0 => Language.ChineseTraditional,
                3 => Language.ChineseSimplified,
                4 => Language.ChineseTraditional,
                5 => Language.French,
                6 => Language.PortugueseBrazil,
                7 => Language.Czech,
                _ => Language.Unknown
            };
        }

        /// <summary>
        /// Auto detects the language of the word list.
        /// </summary>
        /// <param name="sentence">The sentence to detect language from.</param>
        /// <returns>The language.</returns>
        private static Language AutoDetectLanguage(string sentence)
        {
            string[] words = sentence.Split(' ', '　'); //normal space and JP space

            return AutoDetectLanguage(words);
        }

        /// <summary>
        /// Gets the name of the word list.
        /// </summary>
        /// <returns>A string.</returns>
        public override string ToString()
        {
            return _name;
        }

        /// <summary>
        /// Gets the words of the word list.
        /// </summary>
        /// <returns>The words.</returns>
        public IEnumerable<string> GetWords()
        {
            return _words;
        }

        /// <summary>
        /// Gets the words for the given indices.
        /// </summary>
        /// <param name="indices">The indices.</param>
        /// <returns>An array of strings.</returns>
        public string[] GetWords(IEnumerable<int> indices)
        {
            return
                indices
                .Select(GetWordAtIndex)
                .ToArray();
        }

        /// <summary>
        /// Gets the sentence for the given indices.
        /// </summary>
        /// <param name="indices">The indices.</param>
        /// <returns>A string.</returns>
        public string GetSentence(IEnumerable<int> indices)
        {
            return string.Join(Space.ToString(), GetWords(indices));

        }

        /// <summary>
        /// Converts the array of strings to indices.
        /// </summary>
        /// <param name="words">The words.</param>
        /// <returns>The indices of the words.</returns>
        /// <exception cref="FormatException">Thrown when a word is not in the word list.</exception>
        public int[] ToIndices(string[] words)
        {
            int[] indices = new int[words.Length];
            for (int i = 0; i < words.Length; i++)
            {
                if (!WordExists(words[i], out int idx))
                {
                    throw new FormatException("Word " + words[i] + " is not in the wordlist for this language, cannot continue to rebuild entropy from wordlist");
                }
                indices[i] = idx;
            }
            return indices;
        }

        /// <summary>
        /// Converts the given array of integers to a bit array.
        /// </summary>
        /// <param name="values">The array of integers to convert.</param>
        /// <returns>The bit array.</returns>
        /// <exception cref="ArgumentException">Thrown when values are invalid.</exception>
        public static BitArray ToBits(int[] values)
        {
            if (values.Any(v => v >= 2048))
                throw new ArgumentException("values should be between 0 and 2048", nameof(values));
            BitArray result = new(values.Length * 11);
            int i = 0;
            foreach (int val in values)
            {
                for (int p = 0; p < 11; p++)
                {
                    bool v = (val & (1 << (10 - p))) != 0;
                    result.Set(i, v);
                    i++;
                }
            }
            return result;
        }
    }
}