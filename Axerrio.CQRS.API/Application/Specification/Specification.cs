using Axerrio.CQRS.API.Application.Query;
using EnsureThat;
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
        bool HasPredicate { get;  }
        Expression<Func<T, bool>> Predicate { get; set; }
        ISpecification<T> And(Expression<Func<T, bool>> predicate);
        ISpecification<T> Or(Expression<Func<T, bool>> predicate);

        bool HasSelector { get; }
        ISpecification<T> AddSelector<TResult>(Expression<Func<T, TResult>> selector);
        ISpecification<T> AddSelector(MethodCallExpression selector);
        (MethodInfo Method, LambdaExpression LambdaExpression) Selector { get; }

        bool HasOrder { get; }
        IReadOnlyList<(MethodInfo Method, LambdaExpression Lambda)> Order { get; }
        ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        ISpecification<T> AddOrder(MethodCallExpression keySelector);

        bool HasTake { get; }
        int? Take { get; set; }
        bool HasSkip { get; }
        int? Skip { get; set; }
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

        public bool HasPredicate => _predicate.IsStarted;

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

        public bool HasSelector => _selectorMethod != null;

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

        public bool HasOrder => _order.Count > 0;

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

        #region paging

        private int? _take = null;
        public int? Take
        {
            get => _take;

            set
            {
                if (!value.HasValue)
                {
                    _take = null;
                    return;
                }

                EnsureArg.IsGt(value.Value, 0, nameof(value));

                _take = value;
            }
        }

        public bool HasTake => _take.HasValue;

        private int? _skip = null;
        public int? Skip
        {
            get => _skip;

            set
            {
                if (!value.HasValue)
                {
                    _skip = null;
                    return;
                }

                EnsureArg.IsGte(value.Value, 0, nameof(value));

                _skip = value;
            }
        }

        public bool HasSkip => _take.HasValue;

        #endregion
    }

    public class ConstantExtractor: System.Linq.Expressions.ExpressionVisitor
    {
        private ConstantExpression _expression;

        protected ConstantExtractor()
        {
            _expression = null;
        }

        public static ConstantExpression Extract(MethodCallExpression expression)
        {
            var extractor = new ConstantExtractor();

            extractor.Visit(expression.Arguments[1]);

            return extractor.Expression;
        }

        protected ConstantExpression Expression => _expression;

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _expression = node;

            return node;
        }
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