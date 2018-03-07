using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public class LambdaExtractor: ExpressionVisitor
    {
        private LambdaExpression _expression;

        public LambdaExpression Extract(MethodCallExpression expression)
        {
            _expression = null;

            Visit(expression.Arguments[1]);

            return _expression;
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            _expression = node;

            return node;
        }
    }

    public interface ISpecification
    {
        MethodCallExpression Predicate { get; set; }
        MethodCallExpression Selector { get; set; }

        IList<MethodCallExpression> OrderBy {get; set;}
    }

    public class QuerySpecification<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable, ISpecification
    {
        private readonly IQueryProvider _provider;
        private Expression _expression;

        public QuerySpecification()
        {
            _provider = new QuerySpecificationProvider();
            _expression = Expression.Constant(this);

            OrderBy = new List<MethodCallExpression>();
        }

        public Type ElementType => typeof(T);

        public Expression Expression => _expression;

        public IQueryProvider Provider => _provider;

        public MethodCallExpression Predicate { get; set; }
        public MethodCallExpression Selector { get; set; }
        public IList<MethodCallExpression> OrderBy { get; set; }

        public IEnumerator GetEnumerator()
        {
            return ((IEnumerable) _provider.Execute(_expression)).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>) _provider.Execute(_expression)).GetEnumerator();
        }
    }

    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {
        IQueryProvider provider;
        Expression expression;

        public Query(IQueryProvider provider)
            : this(provider, null)
        {
        }

        public Query(IQueryProvider provider, Type staticType)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            this.provider = provider;
            this.expression = staticType != null ? Expression.Constant(this, staticType) : Expression.Constant(this);
        }

        public Query(QueryProvider provider, Expression expression)
        {
            if (provider == null)
            {
                throw new ArgumentNullException("Provider");
            }
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {
                throw new ArgumentOutOfRangeException("expression");
            }
            this.provider = provider;
            this.expression = expression;
        }

        public Expression Expression
        {
            get { return this.expression; }
        }

        public Type ElementType
        {
            get { return typeof(T); }
        }

        public IQueryProvider Provider
        {
            get { return this.provider; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        public override string ToString()
        {
            if (this.expression.NodeType == ExpressionType.Constant &&
                ((ConstantExpression)this.expression).Value == this)
            {
                return "Query(" + typeof(T) + ")";
            }
            else
            {
                return this.expression.ToString();
            }
        }

        //public string QueryText
        //{
        //    get
        //    {
        //        IQueryText iqt = this.provider as IQueryText;
        //        if (iqt != null)
        //        {
        //            return iqt.GetQueryText(this.expression);
        //        }
        //        return "";
        //    }
        //}
    }
}
