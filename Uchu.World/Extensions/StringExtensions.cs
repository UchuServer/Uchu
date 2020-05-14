using System.Collections.Generic;

namespace Uchu.World
{
    public static class StringExtensions
    {
        public static IEnumerable<int> InterpretCollection(this string @this)
        {
            if (string.IsNullOrWhiteSpace(@this)) yield break;

            @this = @this.Replace(" ", "");

            foreach (var entry in @this.Split(','))
            {
                yield return int.Parse(entry);
            }
        }
    }
}