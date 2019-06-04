using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TinyDb.Querying;
using TinyDb.Structure;

namespace TinyDb
{
    public partial class DbSet<T> : IQueryable<T>
        where T : IDbEntry
    {
        public DbSet(string tableName)
        {
            Provider = new QueryProvider();
            Expression = Expression.Constant(this);
            TableName = tableName;

            if (File.Exists($"{tableName}.index"))
            {
                var vt = File.ReadAllText($"{tableName}.index")
                    .ParseJson<VirtualTree>();
                root = null;
                root = ConvertBack(vt);
            }
        }

        public string TableName { get; }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public void Insert(T entity)
        {
            var node = FindNode(root, entity.PrimaryKey);
            Insert(node, entity);
            File.WriteAllText($"{TableName}.index", (root is null ? null : Convert(root)).ToJson());
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return AsEnumerable(root).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return AsEnumerable(root).GetEnumerator();
        }
    }
}
