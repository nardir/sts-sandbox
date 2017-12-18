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
    public class EFSettingConfigurationSource<TContext, TSettingService> : IConfigurationSource
        where TContext : DbContext, ISettingDbContext, new()
        where TSettingService : ISettingService
    {
        protected readonly TContext _context;
        protected readonly Action<DbContextOptionsBuilder<TContext>> _optionsAction;
        protected readonly Func<ISettingService, Task> _seeder;
        protected readonly ILoggerFactory _loggerFactory;

        public EFSettingConfigurationSource(Action<DbContextOptionsBuilder<TContext>> optionsAction, ILoggerFactory loggerFactory, Func<ISettingService, Task> seeder = null)
        {
            _optionsAction = EnsureArg.IsNotNull(optionsAction, nameof(optionsAction));
            _loggerFactory = EnsureArg.IsNotNull(loggerFactory, nameof(LoggerFactory));

            _seeder = seeder;
        }

        public EFSettingConfigurationSource(TContext context, ILoggerFactory loggerFactory, Func<ISettingService, Task> seeder = null)
        {
            _context = EnsureArg.IsNotNull(context, nameof(context));
            _loggerFactory = EnsureArg.IsNotNull(loggerFactory, nameof(LoggerFactory));

            _seeder = seeder;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            if (_optionsAction != null)
                return new EFSettingConfigurationProvider<TContext, TSettingService>(_optionsAction, _loggerFactory, _seeder);

            return new EFSettingConfigurationProvider<TContext, TSettingService>(_context, _loggerFactory, _seeder);
        }
    }
}