using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Config.EFJson
{
    public class JsonEFConfigurationValue
    {
        //public int Id { get; private set; }
        public string Key { get; private set; }
        public string Value { get; set; } //Json

        protected JsonEFConfigurationValue() { }

        public JsonEFConfigurationValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public JsonEFConfigurationValue(string key) : this(key, null)
        {
        }
    }
}