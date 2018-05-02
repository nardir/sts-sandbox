using System;
using System.Collections.Generic;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Validation
{
    [Flags]
    public enum SpecificationOptions
    {
        None = 0,

        Filter = 1,
        Ordering = 2,
        Paging = 4,
        Projection = 8,

        All = Filter | Ordering | Paging | Projection
    }

    public class SpecificationValidationSettings
    {
        #region ctor

        public SpecificationValidationSettings()
        {
        }

        #endregion

        public SpecificationOptions AllowedSpecificationOptions { get; set; } = SpecificationOptions.All;
        public SpecificationOptions RequiredSpecificationOptions { get; set; } = SpecificationOptions.None;
    }
}