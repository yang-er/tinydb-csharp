using System;
using System.Collections.Generic;

namespace TinyDb
{
    static class StructureExtensions
    {
        public static void AddRange<T>(this ISet<T> ts, ISet<T> toMerge)
        {
            foreach (var item in toMerge) ts.Add(item);
        }

        public static void ForEach<T>(this IReadOnlyList<T> list, Action<T> action)
        {
            foreach (var item in list) action(item);
        }
    }
}
