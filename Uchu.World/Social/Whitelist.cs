using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uchu.Core.IO;

namespace Uchu.World.Social
{
    public class Whitelist
    {
        private readonly Regex _wordPattern = new Regex(@"([\w\-']+)");
        
        private readonly IFileResources _resources;
        
        private List<string> AcceptedWords { get; }

        private Dictionary<string, List<string>> AcceptedWordsWithMultipleSections { get;  }

        /// <summary>
        /// Creates the whitelist.
        /// </summary>
        public Whitelist(IFileResources resources)
        {
            _resources = resources;
            AcceptedWords = new List<string>();
            AcceptedWordsWithMultipleSections = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Loads the default words.
        /// </summary>
        public async Task LoadDefaultWhitelist()
        {
            await AddWordsFromResource("./chatminus_en_us.txt");
            await AddWordsFromResource("./chatplus_en_us.txt");
        }

        /// <summary>
        /// Adds the words from a resource text file
        /// </summary>
        /// <param name="filePath">Path of the file to use.</param>
        private async Task AddWordsFromResource(string filePath)
        {
            var raw = await _resources.ReadTextAsync(filePath);
            foreach (var word in raw.Split("\r\n"))
            {
                AddWord(word);
            }
        }

        /// <summary>
        /// Adds a word to the whitelist.
        /// </summary>
        /// <remarks>
        /// The word can contain spaces, such as "Starbase 3001".
        /// </remarks>
        /// <param name="word">Word to add.</param>
        public void AddWord(string word)
        {
            // Add the word.
            word = word.ToLower().Trim();
            if (AcceptedWords.Contains(word)) return;
            AcceptedWords.Add(word.ToLower());
            
            // Set up the word if it has a space.
            var wordSections = _wordPattern.Matches(word);
            if (wordSections.Count == 1) return;
            var initialWord = word.Substring(0, wordSections[0].Length);
            if (!AcceptedWordsWithMultipleSections.ContainsKey(initialWord))
            {
                AcceptedWordsWithMultipleSections[initialWord] = new List<string>();
            }
            AcceptedWordsWithMultipleSections[initialWord].Add(word);
        }

        /// <summary>
        /// Checks if a phrase fails the whitelist.
        /// </summary>
        /// <param name="phrase">Phrase to test.</param>
        /// <returns>Array of the indices and lengths of the invalid strings.</returns>
        public (byte, byte)[] CheckPhrase(string phrase)
        {
            phrase = phrase.ToLower();
            
            // Iterate over the phrase.
            var redact = new List<(byte, byte)>();
            var words = _wordPattern.Matches(phrase);
            for (var matchId = 0; matchId < words.Count; matchId++)
            {
                var match = words[matchId];
                var word = match.Value;
                
                // Match the longest word with spaces if possible, then skip that many words.
                if (AcceptedWordsWithMultipleSections.ContainsKey(word))
                {
                    var remainingString = phrase.Substring(match.Index);
                    string longestMatch = default;
                    foreach (var possibleWord in AcceptedWordsWithMultipleSections[word])
                    {
                        if (!remainingString.StartsWith(possibleWord)) continue;
                        if (remainingString.Length != possibleWord.Length && _wordPattern.Matches(remainingString.Substring(possibleWord.Length, 1)).Count != 0) continue;
                        if (longestMatch != default && longestMatch.Length > possibleWord.Length) continue;
                        longestMatch = possibleWord;
                    }

                    if (longestMatch != default)
                    {
                        matchId += _wordPattern.Matches(longestMatch).Count - 1;
                        continue;
                    }
                }

                // Continue to the next word if the word is allowed.
                var allowed = AcceptedWords.Contains(word);
                if (allowed) continue;

                // Add the bad word's position and length.
                var position = (byte) (phrase.Substring(match.Index).IndexOf(word, StringComparison.Ordinal) + match.Index);
                redact.Add((position, (byte) word.Length));
            }

            return redact.ToArray();
        }
    }
}