using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class ODataQueryProvider : QueryProvider, IQueryProvider
    {
        public override object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }

    public class QuerySpecificationProvider : IQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                var constantExpression = methodCallExpression.Arguments[0] as ConstantExpression;
                var querySpecification = constantExpression.Value as QuerySpecification<TElement>;

                switch (methodCallExpression.Method.Name)
                {
                    case "Where":
                        querySpecification.Predicate = methodCallExpression;

                        break;

                    case "Select":
                        querySpecification.Selector = methodCallExpression;

                        break;

                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":

                        querySpecification.OrderBy.Add(methodCallExpression);

                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return querySpecification;
            }

            throw new InvalidOperationException();
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

    public abstract class QueryProvider : IQueryProvider
    {
        public MethodCallExpression Filter { get; private set; }
        public MethodCallExpression Selector { get; private set; }

        public List<MethodCallExpression> Order { get; private set; }

        protected QueryProvider()
        {
            Order = new List<MethodCallExpression>();
        }

        IQueryable<S> IQueryProvider.CreateQuery<S>(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                switch (methodCallExpression.Method.Name)
                {
                    case "Where":
                        Filter = methodCallExpression;

                        break;

                    case "Select":
                        Selector = methodCallExpression;

                        break;

                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":

                        Order.Add(methodCallExpression);

                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }

            return new Query<S>(this, expression);
        }

        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            Type elementType = TypeHelper.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        S IQueryProvider.Execute<S>(Expression expression)
        {
            return (S)this.Execute(expression);
        }

        object IQueryProvider.Execute(Expression expression)
        {
            return this.Execute(expression);
        }

        //public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);
    }
}
