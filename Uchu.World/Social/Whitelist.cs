using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.IO;

namespace Uchu.World.Social
{
    public class Whitelist
    {
        private readonly IFileResources _resources;
        
        public List<string> AcceptedWords { get; }
        
        public Whitelist(IFileResources resources)
        {
            _resources = resources;
            
            AcceptedWords = new List<string>();
        }

        public async Task LoadDefaultWhitelist()
        {
            var raw = await _resources.ReadTextAsync("./chatminus_en_us.txt");
            
            AcceptedWords.AddRange(raw.Split("\r\n").Select(s => s.ToLower()));

            raw = await _resources.ReadTextAsync("./chatplus_en_us.txt");
            
            AcceptedWords.AddRange(raw.Split("\r\n").Select(s => s.ToLower()));
        }

        public (byte, byte)[] CheckPhrase(string phrase)
        {
            phrase = phrase.ToLower();
            
            var redact = new List<(byte, byte)>();

            foreach (var word in phrase.Split(" "))
            {
                var allowed = AcceptedWords.Contains(word);

                if (allowed) continue;

                var position = (byte) phrase.IndexOf(word, StringComparison.Ordinal);
                    
                redact.Add((position, (byte) word.Length));
            }

            return redact.ToArray();
        }
    }
}