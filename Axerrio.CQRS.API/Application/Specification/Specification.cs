using Axerrio.CQRS.API.Application.Query;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Predicate { get; set; }
        ISpecification<T> And(Expression<Func<T, bool>> predicate);
        ISpecification<T> Or(Expression<Func<T, bool>> predicate);

        ISpecification<T> AddSelector<TResult>(Expression<Func<T, TResult>> selector);
        ISpecification<T> AddSelector(MethodCallExpression selector);
        (MethodInfo Method, LambdaExpression LambdaExpression) Selector { get; }

        IReadOnlyList<(MethodInfo Method, LambdaExpression Lambda)> Order { get; }
        ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        ISpecification<T> AddOrder(MethodCallExpression keySelector);
    }

    public class Specification<T> : ISpecification<T>
    {
        private ExpressionStarter<T> _predicate;


        #region ctor

        public Specification()
            : this(null)
        {
        }

        public Specification(Expression<Func<T, bool>> predicate)
            : this(predicate, null)
        {
            
        }

        public Specification(Expression<Func<T, bool>> predicate, MethodCallExpression selector)
        {
            Predicate = predicate;

            AddSelector(selector);
        }

        #endregion

        #region Predicate

        public Expression<Func<T, bool>> Predicate
        {
            get => _predicate;

            set
            {
                _predicate = PredicateBuilder.New<T>(true);

                if (value != null)
                    _predicate.Start(value);
            }
        }

        public static implicit operator Expression<Func<T, bool>>(Specification<T> right)
        {
            return right.Predicate;
        }

        public static implicit operator Func<T, bool>(Specification<T> right)
        {
            return right.Predicate.Compile();
        }

        public static implicit operator Specification<T>(Expression<Func<T, bool>> right)
        {
            return new Specification<T>(right);
        }

        public ISpecification<T> And(Expression<Func<T, bool>> predicate)
        {
            _predicate = _predicate.And(predicate);

            return this;
        }

        public ISpecification<T> Or(Expression<Func<T, bool>> predicate)
        {
            _predicate = _predicate.Or(predicate);

            return this;
        }

        #endregion

        #region selector

        private LambdaExpression _selectorLambda;
        private MethodInfo _selectorMethod;

        protected ISpecification<T> AddSelector(LambdaExpression selectorLambda, MethodInfo selectorMethod)
        {
            _selectorLambda = selectorLambda;
            _selectorMethod = selectorMethod;

            return this;
        }

        public ISpecification<T> AddSelector<TResult>(Expression<Func<T, TResult>> selector)
        { 
            if (selector == null)
            {
                return AddSelector(null, null);
            }

            var selectorMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(T), selector.Body.Type);

            return AddSelector(selector, selectorMethod);
        }

        public ISpecification<T> AddSelector(MethodCallExpression selector)
        {
            if (selector == null)
            {
                return AddSelector(null, null);
            }

            if (selector.Method.Name != ExpressionHelperMethods.QueryableSelectGeneric.Name)
                    throw new InvalidOperationException();

            if (!typeof(IQueryable<T>).IsAssignableFrom(selector.Arguments[0].Type))
                throw new InvalidOperationException();

            return AddSelector(LambdaExtractor.Extract(selector), selector.Method);
        }

        //https://blogs.msdn.microsoft.com/mazhou/2017/05/26/c-7-series-part-1-value-tuples/
        public (MethodInfo Method, LambdaExpression LambdaExpression) Selector => (Method: _selectorMethod, LambdaExpression: _selectorLambda);

        #endregion

        #region orderby

        private readonly List<(MethodInfo Method, LambdaExpression Lambda)> _order = new List<(MethodInfo Method, LambdaExpression Lambda)>();

        public IReadOnlyList<(MethodInfo Method, LambdaExpression Lambda)> Order => _order.AsReadOnly();

        public ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            //check null

            var orderMethod = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(typeof(T), typeof(TKey));

            return AddOrder(orderMethod, keySelector);
        }

        public ISpecification<T> AddOrder(MethodCallExpression keySelector)
        {
            //check null
            //Check allowed methods

            var keySelectorLambda = LambdaExtractor.Extract(keySelector);

            return AddOrder(keySelector.Method, keySelectorLambda);
        }

        protected ISpecification<T> AddOrder(MethodInfo orderMethod, LambdaExpression keySelector)
        {
            _order.Add((Method: orderMethod, Lambda: keySelector));

            return this;
        }

        #endregion
    }

    public class LambdaExtractor : System.Linq.Expressions.ExpressionVisitor
    {
        private LambdaExpression _expression;

        protected LambdaExtractor()
        {
            _expression = null;
        }

        public static LambdaExpression Extract(MethodCallExpression expression)
        {
            var extractor = new LambdaExtractor();

            extractor.Visit(expression.Arguments[1]);

            return extractor.Expression;
        }

        protected LambdaExpression Expression => _expression;

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _expression = node;

            return node;
        }
    }
}