using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.CQRS.API.Application.Filters
{
    public class Filter2<T>
    {
        protected Filter2()
        {
        }

        public Filter2(string name)
        {
            Name = name;
        }

        public static Filter2<T> FromFilterDefinition(FilterDefinition2 filterDefinition)
        {
            var filter = JsonConvert.DeserializeObject<Filter2<T>>(filterDefinition.Filter);

            return filter;
        }

        public FilterDefinition2 ToFilterDefinition()
        {
            var filter = JsonConvert.SerializeObject(this);
            var context = Context.Name;

            var filterDefinition = new FilterDefinition2(Name, context, filter);

            return filterDefinition;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }

        [JsonIgnore]
        public Type Context => typeof(T);
    }

    public class FilterDefinition2
    {
        protected FilterDefinition2()
        {
        }

        public FilterDefinition2(string name, string context, string filter)
        {
            Name = name;
            Context = context;
            Filter = filter;
        }

        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Context { get; private set; }
        public string Filter { get; private set; }
    }
}