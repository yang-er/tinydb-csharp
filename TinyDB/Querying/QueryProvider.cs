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
            Console.WriteLine(expression.NodeType);

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

            return new Segment(int.MinValue, int.MaxValue, true, true);
        }

        private static (Segment, LambdaExpression) Generate(Expression expression)
        {
            if (expression is UnaryExpression unary)
                if (unary.Operand is LambdaExpression lambda)
                    return (GenerateInner(lambda.Body), lambda);
            throw new NotImplementedException();
        }

        private static IEnumerable ExecuteForce(IQueryable query, Segment seg, Delegate predicate)
        {
            var item = query.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(m => m.Name == "FindBySegment");
            return (IEnumerable)item.Invoke(query, new object[] { seg, predicate });
        }

        private static IEnumerable ConvertQueryableToEnumerable(IEnumerable callsite, MethodCallExpression mce)
        {
            var callArgs = new object[mce.Arguments.Count];
            callArgs[0] = callsite;

            var unbuiltArgs = mce.Arguments.Skip(1).ToArray();
            for (int i = 0; i < unbuiltArgs.Length; i++)
            {
                if (unbuiltArgs[i] is UnaryExpression unary)
                {
                    if (unary.Operand is LambdaExpression lambda)
                    {
                        callArgs[i + 1] = lambda.Compile();
                    }
                }
                else if (unbuiltArgs[i] is LambdaExpression lambda)
                {
                    callArgs[i + 1] = lambda.Compile();
                }
                else if (unbuiltArgs[i] is ConstantExpression constexp)
                {
                    callArgs[i + 1] = constexp.Value;
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            var ga = mce.Method.GetGenericArguments();
            var methods = typeof(Enumerable).GetMethods();
            var method = methods
                .Where(m => mce.Method.Name == m.Name
                         && ga.Length == m.GetGenericArguments().Length
                         && m.GetParameters().Length == callArgs.Length)
                .Select(m => m.MakeGenericMethod(ga))
                .FirstOrDefault() ?? throw new EntryPointNotFoundException(mce.Method.Name);

            return (IEnumerable)method.Invoke(null, callArgs);
        }

        public object Execute(Expression expression)
        {
            var segment = new Segment(int.MinValue, int.MaxValue, true, true);
            Type sourceType = null;
            LambdaExpression whereArgs = null;
            MethodInfo predicateCallSite = null;
            Type predicateType = null;
            Func<Expression, (IEnumerable, bool)> solve = null;
            
            solve = exp =>
            {
                if (exp is MethodCallExpression mce)
                {
                    var res = solve(mce.Arguments.First());

                    if (mce.Method.Name == "Where" && res.Item2)
                    {
                        var (newseg, newlambda) = Generate(mce.Arguments.Last());
                        segment = segment.Intersect(newseg);

                        var toCall = Expression.Parameter(sourceType, "o");
                        var lastPredicate = Expression.Constant(whereArgs.Compile(), predicateType);
                        var newlyPredicate = Expression.Constant(newlambda.Compile(), predicateType);
                        var lastPredicateCall = Expression.Call(lastPredicate, predicateCallSite, toCall);
                        var newlyPredicateCall = Expression.Call(newlyPredicate, predicateCallSite, toCall);
                        var andAlsoPredicate = Expression.AndAlso(lastPredicateCall, newlyPredicateCall);
                        whereArgs = Expression.Lambda(andAlsoPredicate, toCall);
                        return res;
                    }
                    else
                    {
                        if (res.Item1 is IQueryable q)
                        {
                            res.Item1 = ExecuteForce(q, segment, whereArgs.Compile());
                            Console.WriteLine("Warning! " + mce.Arguments.First() + " has been calculated. Next operations will be executed in LINQ to Objects.");
                        }

                        res.Item1 = ConvertQueryableToEnumerable(res.Item1, mce);
                        return (res.Item1, false);
                    }
                }
                else if (exp is ConstantExpression ce)
                {
                    sourceType = ce.Type.GetGenericArguments().First();
                    whereArgs = Expression.Lambda(Expression.Constant(true, typeof(bool)), Expression.Parameter(sourceType, "o"));
                    predicateType = typeof(Func<,>).MakeGenericType(sourceType, typeof(bool));
                    predicateCallSite = predicateType.GetMethod("Invoke");
                    return ((IQueryable)ce.Value, true);
                }
                else
                {
                    throw new NotImplementedException();
                }
            };

            var (result, lastcheck) = solve(expression);
            if (result is IQueryable qq)
                result = ExecuteForce(qq, segment, whereArgs.Compile());
            return result;
        }

        public TResult Execute<TResult>(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
