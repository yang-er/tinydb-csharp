using System.Collections.Generic;

namespace TinyDb
{
    static class StructureExtensions
    {
        public static void AddRange<T>(this ISet<T> ts, ISet<T> toMerge)
        {
            foreach (var item in toMerge) ts.Add(item);
        }
    }
}
