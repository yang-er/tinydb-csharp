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
            return new MyQueryable<TElement>(this, expression);
        }

        public object Execute(Expression expression)
        {
            if (expression is MethodCallExpression mce)
            {
                if (mce.Method.Name == "Where")
                {

                    throw new NotImplementedException();
                }
                else if (mce.Method.Name == "Select")
                {

                    throw new NotImplementedException();
                }
                else
                {

                    throw new NotImplementedException();
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
