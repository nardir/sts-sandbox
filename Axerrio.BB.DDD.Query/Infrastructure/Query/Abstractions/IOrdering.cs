using System.Linq.Expressions;
using System.Reflection;

namespace Axerrio.BB.DDD.Infrastructure.Query.Abstractions
{
    public interface IOrdering
    {
        LambdaExpression KeySelectorLambda { get; set; }
        bool Ascending { get; set; }
        MemberInfo KeySelectorMember { get; set; }
    }
}
