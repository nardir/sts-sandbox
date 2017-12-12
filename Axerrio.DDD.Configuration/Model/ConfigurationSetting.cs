using Axerrio.DDD.BuildingBlocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Model
{
    public class ConfigurationSetting: Entity<int>
    {
        public ConfigurationSection Section { get; private set; }

        public string Key { get; private set; }

        private object _value;

        public string JsonValue { get; private set; }

        public virtual string ValueType { get; private set; }

        protected ConfigurationSetting()
        {
        }

        protected ConfigurationSetting(ConfigurationSection section, string key)
        {
            Section = section;
            Key = key;
        }

        public static ConfigurationSetting Create<T>(ConfigurationSection section, string key, T value)
        {
            var setting = new ConfigurationSetting(section, key);

            setting.SetValue(value);

            return setting;
        }

        public virtual T GetValue<T>()
        {
            return (T)Convert.ChangeType(GetValue(typeof(T)), typeof(T));
        }

        public virtual object GetValue(Type type)
        {
            if ((_value == null) && !string.IsNullOrEmpty(JsonValue))
            {
                _value = JsonConvert.DeserializeObject(JsonValue, type);
            }
            return _value;
        }

        public virtual void SetValue<T>(T value)
        {
            SetValue(value, typeof(T));
        }

        public virtual void SetValue(object value, Type type)
        {
            if (_value != value)
            {
                _value = value;
                JsonValue = null;
                if (value != null)
                {
                    JsonValue = JsonConvert.SerializeObject(value);
                }
            }

            //var name = value.GetType().AssemblyQualifiedName.Split(Convert.ToChar(","));
            var name = type.AssemblyQualifiedName.Split(Convert.ToChar(","));
            ValueType = $"{name[0].Trim()}, {name[1].Trim()}";
        }
    }
}