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
        //private bool _migrated = false;

        protected readonly ILogger<EFSettingConfigurationProvider<TContext, TSettingService>> _logger = new LoggerFactory().CreateLogger<EFSettingConfigurationProvider<TContext, TSettingService>>();
        protected readonly Action<DbContextOptionsBuilder<TContext>> _optionsAction;
        protected readonly Func<ISettingService, Task> _seeder;

        protected TContext Context { get; set; }
        protected bool IsContextOwner { get; set; } = false;

        public EFSettingConfigurationProvider(Action<DbContextOptionsBuilder<TContext>> optionsAction
            , Func<ISettingService, Task> seeder = null)
        {
            _optionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
            _seeder = seeder;
        }

        public EFSettingConfigurationProvider(TContext context
            , Func<ISettingService, Task> seeder = null)
        {
            Context = context;

            _seeder = seeder;
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

                //if (!_migrated && Context.Database.IsSqlServer())
                //{
                //    Context.Database.Migrate();
                //    _migrated = true;
                //}

                if (_seeder != null)
                {
                    var logger = new LoggerFactory().CreateLogger<TSettingService>();

                    var service = (TSettingService)Activator.CreateInstance(typeof(TSettingService), Context, logger);

                    //await _seeder(service);
                    _seeder(service).Wait();
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
