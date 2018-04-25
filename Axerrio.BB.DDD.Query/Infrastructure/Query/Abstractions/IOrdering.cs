using System.Linq.Expressions;

namespace Axerrio.BB.DDD.Infrastructure.Query.Abstractions
{
    public interface IOrdering
    {
        LambdaExpression KeySelectorLambda { get; set; }
        bool Ascending { get; set; }
        string KeySelector { get; set; }
    }
}
