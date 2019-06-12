using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TinyDb.Indexing;
using TinyDb.Querying;
using TinyDb.Structure;

namespace TinyDb
{
    public class DbSet<T> : BpTree<T>, IQueryable<T> where T : IDbEntry
    {
        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public DbSet(string tableName) : base(tableName)
        {
            Provider = new QueryProvider();
            Expression = Expression.Constant(this);
        }

        internal IEnumerable<T> FindBySegment(Segment sg, Func<T, bool> predicate)
        {
            List<T> result = new List<T>();

            foreach (var i in sg.Segments)
            {
                var node = FindNode(root, i.Left);
                if (node is null) break;

                do
                {
                    foreach (var item in node.Data)
                        if (predicate.Invoke(item))
                            result.Add(item);
                    node = node.RightNode;
                }
                while (node != null && node.Key <= i.Right);
            }
            return result;
        }

        private IEnumerable<T> FetchAll()
        {
            var leaf = FindNode(root, int.MinValue);

            while (leaf != null)
            {
                foreach (var item in leaf.Data)
                    yield return item;
                leaf = leaf.RightNode;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return FetchAll().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return FetchAll().GetEnumerator();
        }
    }
}
