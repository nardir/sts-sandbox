using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Filters
{
    public partial class Filter
    {
        [JsonProperty("FilterRows")]
        private readonly List<FilterRow> _filterRows = new List<FilterRow>();

        [JsonIgnore]
        public IReadOnlyCollection<FilterRow> FilterRows => _filterRows;

        public FilterRow AddFilterRow()
        {
            int count = _filterRows.Count;

            var filterRow = new FilterRow(count);

            _filterRows.Add(filterRow);

            return filterRow;
        }

        public Filter AddFilterCondition(FilterRow filterRow, string condition)
        {
            EnsureArg.IsNotNullOrWhiteSpace(condition);

            var filterCondition = new FilterCondition(condition);

            filterCondition.Validate(this);

            if (!_filterRows.Any(r => r.Equals(filterRow)))
                throw new InvalidOperationException();

            filterRow.AddFilterCondition(filterCondition);

            return this;
        }

        public LambdaExpression ParseFilterCondition(FilterCondition filterCondition)
        {
            return DynamicExpressionParser.ParseLambda(Context, typeof(bool), filterCondition.Condition);
        }
    }

    public class FilterRow
    {
        [JsonProperty("FilterConditions")]
        private readonly List<FilterCondition> _filterConditions = new List<FilterCondition>();

        [JsonIgnore]
        public IReadOnlyCollection<FilterCondition> FilterConditions => _filterConditions;

        public FilterRow(int order)
        {
            Order = order;
        }

        public int Order { get; private set; }

        public void AddFilterCondition(FilterCondition filterCondition)
        {
            _filterConditions.Add(filterCondition);
        }
    }

    public class FilterCondition
    {
        public FilterCondition(string condition)
        {
            Condition = condition;
        }

        public string Condition { get; private set; }

        public void Validate(Filter filter)
        {
            filter.ParseFilterCondition(this);
        }
    }
}
