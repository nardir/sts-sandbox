using Axerrio.CQRS.API.Application.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public class ODataQueryable<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable, IQueryProvider
    {
        private readonly Expression _expression;
        private readonly ISpecification<T> _specification;

        public ODataQueryable()
            : this(new Specification<T>())
        {
        }

        public ODataQueryable(ISpecification<T> specification)
        {
            _expression = Expression.Constant(this);

            _specification = specification;
        }

        #region IQueryable

        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => this;

        #endregion

        #region IEnumerable/IEnumerable<T>
        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>) Execute(_expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) Execute(_expression)).GetEnumerator();
        }

        #endregion


        #region IQueryProvider

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                switch (methodCallExpression.Method.Name)
                {
                    case "Where":
                        _specification.Predicate = (Expression<Func<T, bool>>)LambdaExtractor.Extract(methodCallExpression);

                        break;

                    case "Select":
                        _specification.AddSelector(methodCallExpression);

                        break;

                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":

                        _specification.AddOrder(methodCallExpression);

                        break;

                    default:
                        throw new InvalidOperationException();
                }

                return this as IQueryable<TElement>;
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

        #endregion
    }
}
