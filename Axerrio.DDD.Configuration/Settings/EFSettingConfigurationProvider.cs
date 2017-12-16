using EnsureThat;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.DDD.Configuration.Settings
{
    public class EFSettingConfigurationProvider<TContext>: ConfigurationProvider
        where TContext : DbContext, ISettingDbContext, new()
    {        
        protected readonly ILogger<EFSettingConfigurationProvider<TContext>> _logger = new LoggerFactory().CreateLogger<EFSettingConfigurationProvider<TContext>>();
        protected readonly Action<DbContextOptionsBuilder<TContext>> OptionsAction;
        protected TContext Context { get; set; }
        protected bool IsContextOwner { get; set; } = false;

        public EFSettingConfigurationProvider(Action<DbContextOptionsBuilder<TContext>> optionsAction)
        {
            OptionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
        }

        public EFSettingConfigurationProvider(TContext context)
        {
            Context = context;
        }

        public override void Load()
        {
            try
            {
                Data = new Dictionary<string, string>();

                if (Context == null)
                {
                    if (OptionsAction != null)
                    {
                        var builder = new DbContextOptionsBuilder<TContext>();

                        OptionsAction(builder);

                        Context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                    }
                    else
                        Context = new TContext();

                    IsContextOwner = true;
                }

                if (Context.Database.IsSqlServer())
                    Context.Database.Migrate();

                var settings = Context.Settings.AsNoTracking().ToList();

                foreach (var setting in settings)
                {
                    var configSettings = JsonConfigurationParser.Parse(setting.Key, setting.Value).AsEnumerable();

                    foreach(var configSetting in configSettings)
                    {
                        AddSetting(configSetting.Key, configSetting.Value);
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

        protected void AddSetting(string key, string value)
        {
            if (!Data.ContainsKey(key))
                Data.Add(key, value);
        }
    }
}
