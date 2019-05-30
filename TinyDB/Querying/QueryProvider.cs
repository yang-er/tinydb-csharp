using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TinyDb.Querying
{
    internal class QueryProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            try
            {
                return (IQueryable)Activator.CreateInstance(
                    type: typeof(MyQueryable<>).MakeGenericType(expression.Type),
                    args: new object[] { this, expression }
                    );
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            System.Diagnostics.Debug.Assert(typeof(TElement) == expression.Type);
            return new MyQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
