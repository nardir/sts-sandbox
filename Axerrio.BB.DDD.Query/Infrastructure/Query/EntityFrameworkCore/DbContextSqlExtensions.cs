using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.Sql;
using Microsoft.EntityFrameworkCore.Storage;
using Remotion.Linq;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.EntityFrameworkCore
{
    public static class DbContextSqlExtensions
    {
        public static (SelectExpression, IReadOnlyDictionary<string, object>) Compile(this DbContext context, Expression linqExpression)
        {
            QueryContext queryContext = context.GetService<IQueryContextFactory>().Create();
            IEvaluatableExpressionFilter evaluatableExpressionFilter = context.GetService<IEvaluatableExpressionFilter>();

            linqExpression = new ParameterExtractingExpressionVisitor(
                evaluatableExpressionFilter: evaluatableExpressionFilter,
                parameterValues: queryContext,
                logger: context.GetService<IDiagnosticsLogger<DbLoggerCategory.Query>>(),
                parameterize: true).ExtractParameters(linqExpression);

            QueryParser queryParser = new QueryParser(new ExpressionTreeParser(
                nodeTypeProvider: context.GetService<INodeTypeProviderFactory>().Create(),
                processor: new CompoundExpressionTreeProcessor(new IExpressionTreeProcessor[]
                {
                new PartialEvaluatingExpressionTreeProcessor(evaluatableExpressionFilter),
                new TransformingExpressionTreeProcessor(ExpressionTransformerRegistry.CreateDefault())
                })));

            QueryModel queryModel = queryParser.GetParsedQuery(linqExpression);

            Type resultType = queryModel.GetResultType();
            if (resultType.IsConstructedGenericType && (resultType.GetGenericTypeDefinition() == typeof(IQueryable<>) || resultType.GetGenericTypeDefinition() == typeof(EntityQueryable<>)))
            {
                resultType = resultType.GenericTypeArguments.Single();
            }

            QueryCompilationContext compilationContext = context.GetService<IQueryCompilationContextFactory>()
                .Create(async: false);

            RelationalQueryModelVisitor queryModelVisitor = (RelationalQueryModelVisitor)compilationContext
                .CreateQueryModelVisitor();

            queryModelVisitor.GetType()
                .GetMethod(nameof(RelationalQueryModelVisitor.CreateQueryExecutor))
                .MakeGenericMethod(resultType)
                .Invoke(queryModelVisitor, new object[] { queryModel });

            SelectExpression databaseExpression = queryModelVisitor.TryGetQuery(queryModel.MainFromClause);
            databaseExpression.QuerySource = queryModel.MainFromClause;

            return (databaseExpression, queryContext.ParameterValues);
        }

        public static IRelationalCommand Generate(
                                 this DbContext context,
                                 SelectExpression databaseExpression,
                                 IReadOnlyDictionary<string, object> parameters = null)
        {
            IQuerySqlGeneratorFactory sqlGeneratorFactory = context.GetService<IQuerySqlGeneratorFactory>();

            IQuerySqlGenerator sqlGenerator = sqlGeneratorFactory.CreateDefault(databaseExpression);

            return sqlGenerator.GenerateSql(parameters ?? new Dictionary<string, object>());
        }

        public static IRelationalCommand GetRelationalCommand<TEntity>(this DbContext context, IQueryable<TEntity> queryable)
        {
            EnsureArg.IsNotNull(queryable, nameof(queryable));

            Expression linqExpression = queryable.Expression;

            (SelectExpression DatabaseExpression, IReadOnlyDictionary<string, object> Parameters) compilation = context.Compile(linqExpression);

            var relationalCommand = context.Generate(compilation.DatabaseExpression, compilation.Parameters);

            return relationalCommand;
        }
    }
}
