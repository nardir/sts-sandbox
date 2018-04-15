using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Filters
{
    public interface IFilter
    {
        int Key { get; set; }
        string Name { get; set; }
        LambdaExpression Predicate { get; }
        void Where(string predicate);
        Type Context { get; }
    }

    public class Filter<T> : IFilter
        where T : class
    {
        public int Key { get; set; }
        public string Name { get; set; }

        [JsonIgnore]
        public LambdaExpression Predicate => DynamicExpressionParser.ParseLambda(Context, typeof(bool), _predicate);

        [JsonProperty(PropertyName = "Predicate")]
        private string _predicate;

        public void Where(string predicate)
        {
            _predicate = predicate;
        }

        [JsonIgnore]
        public Type Context => typeof(T);
    }

    public partial class Filter //: IFilter
    {
        public Filter(Type context)
        {
            Context = context;
        }

        //[JsonIgnore]
        public Type Context { get; private set; }

        public int Key { get; set; }
        public string Name { get; set; }

        //[JsonIgnore]
        //public LambdaExpression Predicate => DynamicExpressionParser.ParseLambda(Context, typeof(bool), _predicate);

        //[JsonProperty(PropertyName = "Predicate")]
        //private string _predicate;

        //public void Where(string predicate)
        //{
        //    _predicate = predicate;
        //}
    }

    public class FilterDefinition
    {
        public FilterDefinition(Filter filter)
        {
            Key = filter.Key;
            Name = filter.Name;
            Context = filter.Context.AssemblyQualifiedName;
            FilterJson = JsonConvert.SerializeObject(filter);
        }

        public int Key { get; private set; }
        public string Name { get; private set; }
        public string Context { get; private set; }
        public string FilterJson { get; private set; }

        public Filter Filter => (Filter)JsonConvert.DeserializeObject(FilterJson, typeof(Filter));
    }
}
