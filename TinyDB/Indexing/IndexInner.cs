using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyDb.Structure;

namespace TinyDb.Indexing
{
    public class IndexInner<T> : IIndexNode<T> where T : IDbEntry
    {
        readonly List<IIndexNode<T>> childs;

        public bool IsLeaf => false;

        public IIndexNode<T> Parent { get; set; }

        public IReadOnlyList<IIndexNode<T>> Child => childs;

        public int Key { get; set; }

        public IndexInner(List<IIndexNode<T>> chlds)
        {
            childs = chlds;
            Key = chlds.First().Key;
        }

        public void Add(IIndexNode<T> child)
        {
            childs.Add(child);
            childs.Sort((c1, c2) => c1.Key.CompareTo(c2.Key));
            Key = childs.First().Key;
        }

        internal List<IIndexNode<T>> Split()
        {
            var nl = childs.Skip(4).ToList();
            childs.RemoveRange(4, childs.Count - 4);
            return nl;
        }

        public override string ToString()
        {
            return $"Inner, {Key}";
        }
    }
}
