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
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Query
{
    public static class DbContextExtensions
    {
        public static (SelectExpression, IReadOnlyDictionary<string, object>) Compile(
                                this DbContext dbContext, Expression linqExpression)
        {
            QueryContext queryContext = dbContext.GetService<IQueryContextFactory>().Create();

            IEvaluatableExpressionFilter evaluatableExpressionFilter = dbContext.GetService<IEvaluatableExpressionFilter>();
            
            linqExpression = new ParameterExtractingExpressionVisitor(
                evaluatableExpressionFilter: evaluatableExpressionFilter,
                parameterValues: queryContext,
                logger: dbContext.GetService<IDiagnosticsLogger<DbLoggerCategory.Query>>(),
                parameterize: true).ExtractParameters(linqExpression);

            QueryParser queryParser = new QueryParser(new ExpressionTreeParser(
                nodeTypeProvider: dbContext.GetService<INodeTypeProviderFactory>().Create(),
                processor: new CompoundExpressionTreeProcessor(new IExpressionTreeProcessor[]
                {
                new PartialEvaluatingExpressionTreeProcessor(evaluatableExpressionFilter),
                new TransformingExpressionTreeProcessor(ExpressionTransformerRegistry.CreateDefault())
                })));

            QueryModel queryModel = queryParser.GetParsedQuery(linqExpression);

            Type resultType = queryModel.GetResultType();
            if (resultType.IsConstructedGenericType && resultType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            {
                resultType = resultType.GenericTypeArguments.Single();
            }

            QueryCompilationContext compilationContext = dbContext.GetService<IQueryCompilationContextFactory>()
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
                                this DbContext dbContext,
                                SelectExpression databaseExpression,
                                IReadOnlyDictionary<string, object> parameters = null)
        {
            IQuerySqlGeneratorFactory sqlGeneratorFactory = dbContext.GetService<IQuerySqlGeneratorFactory>();
            IQuerySqlGenerator sqlGenerator = sqlGeneratorFactory.CreateDefault(databaseExpression);
            return sqlGenerator.GenerateSql(parameters ?? new Dictionary<string, object>());
        }
    }
}
