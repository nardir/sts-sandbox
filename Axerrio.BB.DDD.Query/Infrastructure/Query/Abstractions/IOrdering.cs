using System.Linq.Expressions;
using System.Reflection;

namespace Axerrio.BB.DDD.Infrastructure.Query.Abstractions
{
    public interface IOrdering
    {
        LambdaExpression KeySelectorLambda { get; }
        bool Ascending { get; }
        //MemberInfo KeySelectorMember { get; }
    }
}
