﻿using Axerrio.CQRS.API.Application.Query;
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

        public ODataQueryable()
            : this(new Specification<T>())
        {
        }

        public ODataQueryable(ISpecification<T> specification)
        {
            _expression = Expression.Constant(this);

            Specification = specification;
        }

        public ISpecification<T> Specification { get; private set; }

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
            return CreateQuery<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                switch (methodCallExpression.Method.Name)
                {
                    case "Where":
                        Specification.Predicate = (Expression<Func<T, bool>>)LambdaExtractor.Extract(methodCallExpression);

                        break;

                    case "Select":
                        Specification.AddSelector(methodCallExpression);

                        break;

                    case "OrderBy":
                    case "ThenBy":
                    case "OrderByDescending":
                    case "ThenByDescending":

                        Specification.AddOrder(methodCallExpression);

                        break;

                    case "Skip":
                        var skip = ConstantExtractor.Extract(methodCallExpression);

                        Specification.Skip = (int)skip.Value;

                        break;

                    case "Take":
                        var take = ConstantExtractor.Extract(methodCallExpression);

                        Specification.Take = (int)take.Value;
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
