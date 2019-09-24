using System;

namespace Uchu.Core
{
    public static class ArrayExtensions
    {
        public static int IndexOf<T>(this T[] @this, T element)
            => Array.IndexOf(@this, element);
    }
}