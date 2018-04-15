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
        void SetPredicate(LambdaExpression predicate);

        bool HasSelector { get; }
        ISpecification<T> AddSelector<TResult>(Expression<Func<T, TResult>> selector);
        ISpecification<T> AddSelector(MethodCallExpression selector);
        (MethodInfo Method, LambdaExpression Lambda) Selector { get; }

        bool HasOrder { get; }
        IReadOnlyList<(MethodInfo Method, LambdaExpression Lambda)> Order { get; }
        ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector);
        ISpecification<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
        ISpecification<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector);
        ISpecification<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector);
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
            : this(default(Expression<Func<T, bool>>))
        {
        }

        public Specification(IDictionary<string, string> specification)
            : this(default(Expression<Func<T, bool>>))
        {
            if (specification != null)
            {
                var orderbyRaw = specification["$orderby"];
                var desc = new string[] { "DESC" };

                var orderby = orderbyRaw.Split(',');

                foreach(var o in orderby)
                {
                    bool asc = true;

                    if (o.IndexOf("Desc", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        asc = false;
                    }
                }
            }
        }

        public Specification(Expression<Func<T, bool>> predicate)
            : this(predicate, null)
        {
            
        }

        public Specification(Expression<Func<T, bool>> predicate, MethodCallExpression selector)
        {
            Predicate = predicate;

            _order = new List<(MethodInfo Method, LambdaExpression Lambda)>();

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

        public void SetPredicate(LambdaExpression predicate)
        {
            Predicate = (Expression<Func<T, bool>>) predicate;
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

            //var selectorMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(T), selector.Body.Type);
            var selectorMethod = ExpressionHelperMethods.QueryableSelectGeneric.MakeGenericMethod(typeof(T), typeof(TResult));

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
        public (MethodInfo Method, LambdaExpression Lambda) Selector => (Method: _selectorMethod, Lambda: _selectorLambda);

        #endregion

        #region orderby

        private readonly List<(MethodInfo Method, LambdaExpression Lambda)> _order;

        public bool HasOrder => _order.Count > 0;

        public IReadOnlyList<(MethodInfo Method, LambdaExpression Lambda)> Order => _order.AsReadOnly();

        public ISpecification<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderMethod = ExpressionHelperMethods.QueryableOrderByGeneric.MakeGenericMethod(typeof(T), typeof(TKey));

            return AddOrder(orderMethod, keySelector);
        }

        public ISpecification<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderMethod = ExpressionHelperMethods.QueryableOrderByDescendingGeneric.MakeGenericMethod(typeof(T), typeof(TKey));

            return AddOrder(orderMethod, keySelector);
        }

        public ISpecification<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderMethod = ExpressionHelperMethods.QueryableThenByGeneric.MakeGenericMethod(typeof(T), typeof(TKey));

            return AddOrder(orderMethod, keySelector);
        }

        public ISpecification<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
        {
            var orderMethod = ExpressionHelperMethods.QueryableThenByDescendingGeneric.MakeGenericMethod(typeof(T), typeof(TKey));

            return AddOrder(orderMethod, keySelector);
        }

        public ISpecification<T> AddOrder(MethodCallExpression keySelector)
        {
            EnsureArg.IsNotNull(keySelector, nameof(keySelector));

            string[] allowedMethods = new string[] { "OrderBy", "ThenBy", "OrderByDescending", "ThenByDescending" };

            if (!allowedMethods.Any(m => m == keySelector.Method.Name))
                throw new InvalidOperationException();

            var keySelectorLambda = LambdaExtractor.Extract(keySelector);

            return AddOrder(keySelector.Method, keySelectorLambda);
        }

        protected ISpecification<T> AddOrder(MethodInfo orderMethod, LambdaExpression keySelector)
        {
            EnsureArg.IsNotNull(keySelector, nameof(keySelector));
            EnsureArg.IsNotNull(orderMethod, nameof(orderMethod));

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

        public bool HasSkip => _skip.HasValue;

        #endregion
    }

    public class ConstantValueExtractor: System.Linq.Expressions.ExpressionVisitor
    {
        private ConstantExpression _constantExpression;
        private MemberExpression _memberExpression;

        protected ConstantValueExtractor()
        {
            _memberExpression = null;
        }

        //public static ConstantExpression Extract(MethodCallExpression expression)
        public static T Extract<T>(MethodCallExpression expression)
        {
            var extractor = new ConstantValueExtractor();

            extractor.Visit(expression.Arguments[1]);

            return extractor.GetValue<T>();
        }

        //protected ConstantExpression Expression => _expression;
        protected MemberExpression MemberExpression => _memberExpression;
        protected ConstantExpression ConstantExpression => _constantExpression;

        protected override Expression VisitMember(MemberExpression node)
        {
            _memberExpression = node;

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _constantExpression = node;

            return node;
        }

        protected T GetValue<T>()
        {
            var value = _constantExpression.Value;

            var t = _constantExpression.Type;
            var fieldName = _memberExpression.Member.Name;

            var fieldInfo = t.GetField(fieldName) ?? t.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                value = fieldInfo.GetValue(value);
            }
            else
            {
                PropertyInfo propInfo = t.GetProperty(fieldName) ?? t.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (propInfo != null)
                {
                    value = propInfo.GetValue(value, null);
                }
            }

            return (T)value;
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