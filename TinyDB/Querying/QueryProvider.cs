using System;
using System.Collections;
using System.Collections.Generic;
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

        private static Segment GenerateInner(Expression expression)
        {
            Console.WriteLine(expression.GetType());

            if (expression is BinaryExpression be)
            {
                if (be.NodeType == ExpressionType.AndAlso)
                {
                    return GenerateInner(be.Left).Intersect(GenerateInner(be.Right));
                }
                else if (be.NodeType == ExpressionType.OrElse)
                {
                    return GenerateInner(be.Left).Join(GenerateInner(be.Right));
                }
                else if (be.Left is MemberExpression me && be.Right is ConstantExpression ce)
                {
                    if (me.Member.Name == "Id")
                    {
                        int vv = (int)ce.Value;

                        switch (be.NodeType)
                        {
                            case ExpressionType.Equal:
                                return new Segment(vv, vv, true, true);
                            case ExpressionType.NotEqual:
                                return new Segment(vv, vv, true, true).Reverse();
                            case ExpressionType.LessThan:
                                return new Segment(int.MinValue, vv, true, false);
                            case ExpressionType.LessThanOrEqual:
                                return new Segment(int.MinValue, vv, true, true);
                            case ExpressionType.GreaterThan:
                                return new Segment(vv, int.MaxValue, false, true);
                            case ExpressionType.GreaterThanOrEqual:
                                return new Segment(vv, int.MaxValue, true, true);
                        }
                    }
                }
            }
            else if (expression is UnaryExpression unary)
            {
                if (unary.NodeType == ExpressionType.Not)
                {
                    return GenerateInner(unary.Operand).Reverse();
                }
            }

            throw new NotImplementedException();
        }

        private static Segment Generate(Expression expression)
        {
            if (expression is UnaryExpression unary)
                if (unary.Operand is LambdaExpression lambda)
                    return GenerateInner(lambda.Body);
            throw new NotImplementedException();
        }

        private static IEnumerable<T> ExecuteForceInner<T>(IQueryable<T> query, Segment seg)
        {
            return new T[0];
        }

        private static IEnumerable ExecuteForce(IQueryable query, Segment seg)
        {
            var item = query.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m => m.Name == "FindBySegment");
            return (IEnumerable)item.Invoke(query, new object[] { seg });
        }

        public object Execute(Expression expression)
        {
            var segment = new Segment(int.MinValue, int.MaxValue, true, true);

            Func<Expression, (IEnumerable, bool)> solve = null;
            
            solve = exp =>
            {
                if (exp is MethodCallExpression mce)
                {
                    var res = solve(mce.Arguments.First());

                    if (mce.Method.Name == "Where" && res.Item2)
                    {
                        segment = segment.Intersect(Generate(mce.Arguments.Last()));
                        return res;
                    }
                    else
                    {
                        if (res.Item1 is IQueryable q)
                            res.Item1 = ExecuteForce(q, segment);
                        
                        throw new NotImplementedException();
                    }
                }
                else if (exp is ConstantExpression ce)
                {
                    return ((IQueryable)ce.Value, true);
                }
                else
                {
                    throw new NotImplementedException();
                }
            };

            var (result, _) = solve(expression);
            return result;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
