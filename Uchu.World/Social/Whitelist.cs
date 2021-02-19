using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Uchu.Core.IO;

namespace Uchu.World.Social
{
    public class Whitelist
    {
        private readonly IFileResources _resources;
        
        private List<string> AcceptedWords { get; }
        
        /// <summary>
        /// Creates the whitelist.
        /// </summary>
        public Whitelist(IFileResources resources)
        {
            _resources = resources;
            AcceptedWords = new List<string>();
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
            word = word.ToLower();
            if (AcceptedWords.Contains(word)) return;
            AcceptedWords.Add(word.ToLower());
            
            // Set up the word if it has a space.
            if (word.Contains(" "))
            {
                // TODO: Implement
            }
        }

        /// <summary>
        /// Checks if a phrase fails the whitelist.
        /// </summary>
        /// <param name="phrase">Phrase to test.</param>
        /// <returns>Array of the indices and lengths of the invalid strings.</returns>
        public (byte, byte)[] CheckPhrase(string phrase)
        {
            phrase = phrase.ToLower();
            
            var redact = new List<(byte, byte)>();
            foreach (Match match in new Regex(@"([\w\-']+)").Matches(phrase))
            {
                var word = match.Value;
                var allowed = AcceptedWords.Contains(word);

                if (allowed) continue;

                var position = (byte) phrase.IndexOf(word, StringComparison.Ordinal);
                    
                redact.Add((position, (byte) word.Length));
            }

            return redact.ToArray();
        }
    }
}