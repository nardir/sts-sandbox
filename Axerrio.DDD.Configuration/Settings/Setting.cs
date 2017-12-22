using EnsureThat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public class Setting
    {
        private object _value;

        public string Key { get; private set; }
        public string Value { get; private set; }
        public string ValueType { get; private set; }

        protected Setting() {}

        protected Setting(string key)
        {
            Key = EnsureArg.IsNotNullOrWhiteSpace(key, nameof(key));
        }

        public static Setting Create<T>(string key, T value)
        {
            var setting = new Setting(key);

            setting.SetValue(value);

            return setting;
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
                Value = null;
                if (value != null)
                {
                    Value = JsonConvert.SerializeObject(value);
                }
            }

            var name = type.AssemblyQualifiedName.Split(Convert.ToChar(","));
            ValueType = $"{name[0].Trim()}, {name[1].Trim()}";
        }

        public virtual T GetValue<T>()
        {
            return (T)Convert.ChangeType(GetValue(typeof(T)), typeof(T));
        }

        public virtual object GetValue(Type type)
        {
            if ((_value == null) && !string.IsNullOrEmpty(Value))
            {
                _value = JsonConvert.DeserializeObject(Value, type);
            }
            return _value;
        }

        public virtual object GetValue()
        {
            return GetValue(Type.GetType(ValueType));
        }
    }
}
