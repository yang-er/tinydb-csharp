using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using TinyDb.Querying;

namespace TinyDb
{
    public class DbSet<T> : IQueryable<T>
    {
        public DbSet()
        {
            Provider = new QueryProvider();
            Expression = Expression.Constant(this);
        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public void Insert(T entity)
        {
            throw new NotImplementedException();
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
            return ((IEnumerable<T>)Provider.Execute(Expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();
        }
    }
}
