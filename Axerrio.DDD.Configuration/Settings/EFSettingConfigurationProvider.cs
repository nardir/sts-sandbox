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
    public class EFSettingConfigurationProvider<TContext, TSettingService>: ConfigurationProvider
        where TContext : DbContext, ISettingDbContext, new()
        where TSettingService : ISettingService
    {
        protected readonly ILogger<EFSettingConfigurationProvider<TContext, TSettingService>> _logger;
        protected readonly Action<DbContextOptionsBuilder<TContext>> _optionsAction;
        protected readonly ILoggerFactory _loggerFactory;

        protected TContext Context { get; set; }
        protected bool IsContextOwner { get; set; } = false;

        protected EFSettingConfigurationProvider(ILoggerFactory loggerFactory)
        {
            _loggerFactory = EnsureArg.IsNotNull(loggerFactory, nameof(LoggerFactory));

            _logger = _loggerFactory.CreateLogger<EFSettingConfigurationProvider<TContext, TSettingService>>();
        }

        public EFSettingConfigurationProvider(Action<DbContextOptionsBuilder<TContext>> optionsAction, ILoggerFactory loggerFactory): this(loggerFactory)
        {
            _optionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
        }

        public EFSettingConfigurationProvider(TContext context, ILoggerFactory loggerFactory): this(loggerFactory)
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
                    if (_optionsAction != null)
                    {
                        var builder = new DbContextOptionsBuilder<TContext>();

                        _optionsAction(builder);

                        Context = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);
                    }
                    else
                        Context = new TContext();

                    IsContextOwner = true;
                }

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
