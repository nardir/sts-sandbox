using Axerrio.DDD.Configuration.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration
{
    public class EFConfigurationProvider: ConfigurationProvider
    {
        protected virtual Action<DbContextOptionsBuilder> OptionsAction { get; set; }

        protected virtual ConfigurationContext Context { get; set; }

        protected virtual bool IsContextOwner { get; set; } = false;

        public EFConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        public EFConfigurationProvider(ConfigurationContext context)
        {
            Context = context;
        }

        public override void Load()
        {
            Data = new Dictionary<string, string>();

            if (Context == null)
            {
                if (OptionsAction != null)
                {
                    var builder = new DbContextOptionsBuilder<ConfigurationContext>();

                    OptionsAction(builder);

                    Context = new ConfigurationContext(builder.Options);
                }
                else
                {
                    Context = new ConfigurationContext();
                }

                //if (EnsureCreated) this.Context.Database.EnsureCreated();

                IsContextOwner = true;
            }

            try
            {
                var settings = Context.Settings.AsNoTracking().ToList();

                foreach(var setting in settings)
                {
                    if (this.IsJsonObjectOrArray(setting.JsonValue))
                    {
                        var data = JsonConvert.DeserializeObject(setting.JsonValue);
                        var container = (JContainer)data;
                        AddJObjectToData(setting.Section.Name, container);
                    }
                    else
                    {
                        this.AddSetting(setting.Key, setting.JsonValue);
                    }
                }
            }
            finally
            {
                if (IsContextOwner && Context != null)
                {
                    Context.Dispose();
                    Context = null;
                }
            }
        }

        protected virtual bool IsJsonObjectOrArray(string value)
        {
            value = value.Trim();
            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                return true;
            }
            return false;
        }

        protected virtual void AddJObjectToData(string section, JContainer json)
        {
            foreach (var kvp in (JObject)json)
            {
                if (kvp.Value is JObject)
                {
                    AddJObjectToData($"{section}:{kvp.Key}", (JObject)kvp.Value);
                }
                else if (kvp.Value is JArray)
                {
                    var array = (JArray)kvp.Value;
                    var i = 0;
                    foreach (var item in array.OfType<JObject>())
                    {
                        AddJObjectToData($"{section}:{kvp.Key}:{i}", item);
                        i++;
                    }
                }
                else
                {
                    var value = kvp.Value.Type != JTokenType.Null ? kvp.Value.ToString() : null;
                    AddSetting($"{section}:{kvp.Key}", value);
                }
            }
        }

        protected virtual void AddSetting(string key, string value)
        {
            if (!Data.ContainsKey(key))
                Data.Add(key, value);
        }
    }
}
