//using Axerrio.CQRS.API.Application.Query;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Threading.Tasks;

//namespace Axerrio.CQRS.API.Application.Specification
//{
//    public interface ISpecification<TEntity> //Marker
//    {
//    }

//    public interface ISpecification<TEntity, TSpecification, TExpression>: ISpecification<TEntity>
//        where TSpecification : ISpecification<TEntity, TSpecification, TExpression>
//    {
//        TExpression ToExpression();
//        TSpecification And(TSpecification specification);
//    }

//    public abstract class Specification<TEntity>
//        : ISpecification<TEntity, Specification<TEntity>, Expression<Func<TEntity, bool>>>
//    {
//        public abstract Expression<Func<TEntity, bool>> ToExpression();

//        public Specification<TEntity> And(Specification<TEntity> specification)
//        {
//            return this;
//        }

//        public bool IsSatisfiedBy(TEntity entity)
//        {
//            Func<TEntity, bool> predicate = ToExpression().Compile();

//            return predicate(entity);
//        }
//    }

//    public class SQLFilterExpression
//    {
//        public List<object> Parameters { get; }
//        public string FilterClause { get; }
//    }

//    public abstract class SQLSpecification<TEntity> 
//        : ISpecification<TEntity, SQLSpecification<TEntity>, SQLFilterExpression>
//    {
//        public SQLSpecification<TEntity> And(SQLSpecification<TEntity> specification)
//        {
//            throw new NotImplementedException();
//        }

//        public abstract SQLFilterExpression ToExpression();
//    }

//    public interface IQueries
//    {
//        IEnumerable<dynamic> GetSalesOrders(ISpecification<SalesOrder> specification);
//    }

//    public class SQLQueries : IQueries
//    {
//        public IEnumerable<dynamic> GetSalesOrders(ISpecification<SalesOrder> specification)
//        {
//            var sqlSpecification = specification as SQLSpecification<SalesOrder>;

//            //checked sqlSpecification is Nullable -> ArgumentNullException

//            var expression = sqlSpecification.ToExpression();

//            throw new NotImplementedException();
//        }
//    }
//}
