using System;

namespace Axerrio.BB.DDD.Infrastructure.Query.Validation
{
    [Flags]
    public enum SpecificationOptions
    {
        None = 0,

        Filtering = 1,
        Ordering = 2,
        Paging = 4,
        Projection = 8,

        All = Filtering | Ordering | Paging | Projection
    }
}