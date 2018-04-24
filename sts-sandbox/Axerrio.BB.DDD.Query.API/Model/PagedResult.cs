using Axerrio.BB.DDD.Domain.Abstractions;
using EnsureThat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD.Query.API.Model
{
    public class PagedResult<T> : ValueObject<PagedResult<T>>
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long Count { get; private set; }

        public IEnumerable<T> Data { get; private set; }

        protected PagedResult(int pageIndex, int pageSize, long count, IEnumerable<T> data)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Count = count;
            Data = data;
        }

        public static PagedResult<T> Create(int pageIndex, int pageSize, long count, IEnumerable<T> data)
        {
            EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));
            EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
            EnsureArg.IsGte(count, 0, nameof(count));

            return new PagedResult<T>(pageIndex, pageSize, count, data);
        }

        protected override IEnumerable<object> GetMemberValues()
        {
            yield return PageIndex;
            yield return PageSize;
            yield return Count;
            yield return Data;
        }
    }
}
