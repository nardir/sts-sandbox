using Axerrio.BB.DDD.Infrastructure.Query.Abstractions;
using EnsureThat;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Axerrio.BB.DDD.Infrastructure.Query.Model
{
    public static class PagedEnumerable
    {
        public static PagedEnumerable<TEntity> Create<TEntity>(IEnumerable<TEntity> items, int pageIndex, int pageSize, long itemCount)
        {
            return new PagedEnumerable<TEntity>(items, pageIndex, pageSize, itemCount);
        }

        public static PagedEnumerable<TResult> Create<TResult, TEntity>(IEnumerable<TResult> items, ISpecification<TEntity> specification, long itemCount)
        {
            EnsureArg.IsNotNull(specification, nameof(specification));
            EnsureArg.IsTrue(specification.HasPaging, nameof(specification.HasPaging));

            return Create(items, specification.PageIndex.Value, specification.PageSize.Value, itemCount);
        }
    }

    public class PagedEnumerable<TEntity> //: IEnumerable<TEntity>
    {
        public int PageIndex { get; private set; }

        public int PageSize { get; private set; }

        public long ItemCount { get; private set; }

        public long PageCount { get; private set; }

        public IEnumerable<TEntity> Items { get; private set; }

        #region ctor

        internal PagedEnumerable(IEnumerable<TEntity> items, int pageIndex, int pageSize, long itemCount)
        {
            EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));
            EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
            EnsureArg.IsGte(itemCount, 0, nameof(itemCount));

            PageIndex = pageIndex;
            PageSize = pageSize;
            ItemCount = itemCount;

            PageCount = 0;
            if (ItemCount > 0)
            {
                PageCount = ((ItemCount - 1) / PageSize) + 1;
            }

            Items = items ?? new List<TEntity>();
        }

        #endregion

        #region static

        #region factory

        //public static PagedEnumerable<TEntity> Create(int pageIndex, int pageSize, long itemCount, IEnumerable<TEntity> items)
        //{
        //    EnsureArg.IsGte(pageIndex, 0, nameof(pageIndex));
        //    EnsureArg.IsGt(pageSize, 0, nameof(pageSize));
        //    EnsureArg.IsGte(itemCount, 0, nameof(itemCount));

        //    return new PagedEnumerable<TEntity>(items, pageIndex, pageSize, itemCount);
        //}

        #endregion

        #endregion

        #region IEnumerable

        //public IEnumerator<TEntity> GetEnumerator()
        //{
        //    return Items.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return Items.GetEnumerator();
        //}

        #endregion
    }
}
